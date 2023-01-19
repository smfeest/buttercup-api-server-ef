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

    [Fact]
    public async Task SaveUpgradedPasswordHash_UpdatesHashAndReturnsTrueWhenUserIdAndCurrentHashMatch()
    {
        using var dbContext = this.dbFixture.CreateDbContext();

        try
        {
            var insertedUser = this.sampleDataFactory.BuildUser(); // TODO: Consider creating disposable fixture instead
            insertedUser.PasswordHash = "current-hash";

            dbContext.Users.Add(insertedUser);
            await dbContext.SaveChangesAsync();

            Assert.True(
                await this.passwordAuthenticationQueries.SaveUpgradedPasswordHash(
                    dbContext, insertedUser.Id, "current-hash", "new-hash"));

            dbContext.ChangeTracker.Clear(); // TODO: Consider using new context instead

            var reretrievedUser = await dbContext.Users.FindAsync(insertedUser.Id);

            Assert.Equal("new-hash", reretrievedUser?.PasswordHash);
        }
        finally
        {
            await dbContext.Users.ExecuteDeleteAsync();
        }
    }

    [Fact]
    public async Task SaveUpgradedPasswordHash_DoesNotUpdateHashAndReturnsFalseWhenUserIdDoesNotMatch()
    {
        using var dbContext = this.dbFixture.CreateDbContext();

        try
        {
            var insertedUser = this.sampleDataFactory.BuildUser();
            insertedUser.PasswordHash = "current-hash";

            dbContext.Users.Add(insertedUser);
            await dbContext.SaveChangesAsync();

            Assert.False(
                await this.passwordAuthenticationQueries.SaveUpgradedPasswordHash(
                    dbContext, this.sampleDataFactory.NextInt(), "current-hash", "new-hash"));

            dbContext.ChangeTracker.Clear();

            var reretrievedUser = await dbContext.Users.FindAsync(insertedUser.Id);

            Assert.Equal("current-hash", reretrievedUser?.PasswordHash);
        }
        finally
        {
            await dbContext.Users.ExecuteDeleteAsync();
        }
    }

    [Fact]
    public async Task SaveUpgradedPasswordHash_DoesNotUpdateHashAndReturnsFalseWhenCurrentHashDoesNotMatch()
    {
        using var dbContext = this.dbFixture.CreateDbContext();

        try
        {
            var insertedUser = this.sampleDataFactory.BuildUser();
            insertedUser.PasswordHash = "current-hash";

            dbContext.Users.Add(insertedUser);
            await dbContext.SaveChangesAsync();

            Assert.False(
                await this.passwordAuthenticationQueries.SaveUpgradedPasswordHash(
                    dbContext, insertedUser.Id, "old-hash", "new-hash"));

            dbContext.ChangeTracker.Clear();

            var reretrievedUser = await dbContext.Users.FindAsync(insertedUser.Id);

            Assert.Equal("current-hash", reretrievedUser?.PasswordHash);
        }
        finally
        {
            await dbContext.Users.ExecuteDeleteAsync();
        }
    }

    #endregion
}
