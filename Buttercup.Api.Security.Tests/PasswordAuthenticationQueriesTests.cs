using Buttercup.Api.DbModel;
using Buttercup.Api.TestUtils;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Buttercup.Api.Security;

public sealed class PasswordAuthenticationQueriesTests :
    IClassFixture<DbFixture<PasswordAuthenticationQueriesTests>>
{
    private readonly DbFixture<PasswordAuthenticationQueriesTests> dbFixture;
    private readonly PasswordAuthenticationQueries passwordAuthenticationQueries = new();
    private readonly SampleDataFactory sampleDataFactory = new();

    public PasswordAuthenticationQueriesTests(
        DbFixture<PasswordAuthenticationQueriesTests> databaseFixture) =>
        this.dbFixture = databaseFixture;

    #region FindUserByEmail

    [Fact]
    public async Task FindUserByEmail_ReturnsSpecifiedUser()
    {
        using var dbContext = this.dbFixture.CreateDbContext();

        try
        {
            var users = new[]
            {
                this.sampleDataFactory.BuildUser(),
                this.sampleDataFactory.BuildUser(),
            };

            dbContext.Users.AddRange(users);
            await dbContext.SaveChangesAsync();

            foreach (var user in users)
            {
                Assert.Equivalent(
                    user,
                    await this.passwordAuthenticationQueries.FindUserByEmail(
                        dbContext, user.Email));
            }
        }
        finally
        {
            await dbContext.Users.ExecuteDeleteAsync();
        }
    }

    [Fact]
    public async Task FindUserByEmail_ReturnsNullWhenUserDoesNotExist()
    {
        using var dbContext = this.dbFixture.CreateDbContext();

        Assert.Null(await this.passwordAuthenticationQueries.FindUserByEmail(
            dbContext, "non-existent@example.com"));
    }

    #endregion

    #region SaveUpgradedPasswordHash

    private sealed class SaveUpgradedPasswordHashFixture : IAsyncDisposable
    {
        public const string CurrentHash = "current-hash";
        public const string NewHash = "new-hash";

        private readonly PasswordAuthenticationQueriesTests context;

        public SaveUpgradedPasswordHashFixture(PasswordAuthenticationQueriesTests context)
        {
            this.context = context;

            var user = context.sampleDataFactory.BuildUser();
            user.PasswordHash = CurrentHash;
            this.User = user;
        }

        public User User { get; }

        public async Task InsertUser()
        {
            using var dbContext = this.context.dbFixture.CreateDbContext();
            dbContext.Add(this.User);
            await dbContext.SaveChangesAsync();
        }

        public async Task<User?> RefetchUser()
        {
            using var dbContext = this.context.dbFixture.CreateDbContext();
            return await dbContext.Users.FindAsync(this.User.Id);
        }

        public async ValueTask DisposeAsync()
        {
            using var dbContext = this.context.dbFixture.CreateDbContext();
            await dbContext.Users.ExecuteDeleteAsync();
        }
    }

    [Fact]
    public async Task SaveUpgradedPasswordHash_UpdatesHashAndReturnsTrueWhenUserIdAndCurrentHashMatch()
    {
        await using var fixture = new SaveUpgradedPasswordHashFixture(this);

        await fixture.InsertUser();

        Assert.True(
            await this.passwordAuthenticationQueries.SaveUpgradedPasswordHash(
                this.dbFixture.CreateDbContext(),
                fixture.User.Id,
                SaveUpgradedPasswordHashFixture.CurrentHash,
                SaveUpgradedPasswordHashFixture.NewHash));

        var refetchedUser = await fixture.RefetchUser();

        Assert.Equal(SaveUpgradedPasswordHashFixture.NewHash, refetchedUser?.PasswordHash);
    }

    [Fact]
    public async Task SaveUpgradedPasswordHash_DoesNotUpdateHashAndReturnsFalseWhenUserIdDoesNotMatch()
    {
        await using var fixture = new SaveUpgradedPasswordHashFixture(this);

        await fixture.InsertUser();

        Assert.False(
            await this.passwordAuthenticationQueries.SaveUpgradedPasswordHash(
                this.dbFixture.CreateDbContext(),
                this.sampleDataFactory.NextInt(),
                SaveUpgradedPasswordHashFixture.CurrentHash,
                SaveUpgradedPasswordHashFixture.NewHash));

        var refetchedUser = await fixture.RefetchUser();

        Assert.Equal(SaveUpgradedPasswordHashFixture.CurrentHash, refetchedUser?.PasswordHash);
    }

    [Fact]
    public async Task SaveUpgradedPasswordHash_DoesNotUpdateHashAndReturnsFalseWhenCurrentHashDoesNotMatch()
    {
        await using var fixture = new SaveUpgradedPasswordHashFixture(this);

        await fixture.InsertUser();

        Assert.False(
            await this.passwordAuthenticationQueries.SaveUpgradedPasswordHash(
                this.dbFixture.CreateDbContext(),
                fixture.User.Id,
                "old-hash",
                SaveUpgradedPasswordHashFixture.NewHash));

        var refetchedUser = await fixture.RefetchUser();

        Assert.Equal(SaveUpgradedPasswordHashFixture.CurrentHash, refetchedUser?.PasswordHash);
    }

    #endregion
}
