using Buttercup.Api.TestUtils;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Buttercup.Api.Security;

public sealed class PasswordAuthenticationServiceQueriesTests :
    IClassFixture<DbFixture<PasswordAuthenticationServiceQueriesTests>>, IAsyncLifetime
{
    private readonly DbFixture<PasswordAuthenticationServiceQueriesTests> dbFixture;
    private readonly PasswordAuthenticationServiceQueries queries = new();
    private readonly SampleDataFactory sampleDataFactory = new();

    public PasswordAuthenticationServiceQueriesTests(
        DbFixture<PasswordAuthenticationServiceQueriesTests> databaseFixture) =>
        this.dbFixture = databaseFixture;

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        using var dbContext = this.dbFixture.CreateDbContext();
        await dbContext.Users.ExecuteDeleteAsync();
    }

    #region FindUserByEmail

    [Fact]
    public async Task FindUserByEmail_OnSuccess_ReturnsSpecifiedUser()
    {
        using var dbContext = this.dbFixture.CreateDbContext();

        var users = new[]
        {
            this.sampleDataFactory.BuildUser(),
            this.sampleDataFactory.BuildUser(),
        };

        dbContext.Users.AddRange(users);
        await dbContext.SaveChangesAsync();

        foreach (var user in users)
        {
            Assert.Equal(
                user,
                await this.queries.FindUserByEmail(dbContext, user.Email));
        }
    }

    [Fact]
    public async Task FindUserByEmail_WhenUserDoesNotExist_ReturnsNull()
    {
        using var dbContext = this.dbFixture.CreateDbContext();

        Assert.Null(await this.queries.FindUserByEmail(
            dbContext, "non-existent@example.com"));
    }

    #endregion

    #region SaveUpgradedPasswordHash

    [Fact]
    public async Task SaveUpgradedPasswordHash_OnSuccess_UpdatesHashAndReturnsTrue()
    {
        var userBefore = this.sampleDataFactory.BuildUser() with { PasswordHash = "current-hash" };

        using (var dbContext = this.dbFixture.CreateDbContext())
        {
            dbContext.Users.Add(userBefore);
            await dbContext.SaveChangesAsync();
        }

        using (var dbContext = this.dbFixture.CreateDbContext())
        {
            Assert.True(await this.queries.SaveUpgradedPasswordHash(
                dbContext, userBefore.Id, "current-hash", "new-hash"));
        }

        using (var dbContext = this.dbFixture.CreateDbContext())
        {
            var userAfter = await dbContext.Users.FindAsync(userBefore.Id);
            Assert.Equal(userBefore with { PasswordHash = "new-hash" }, userAfter);
        }
    }

    [Fact]
    public async Task SaveUpgradedPasswordHash_WhenUserIdDoesNotMatch_DoesNotUpdateHashAndReturnsFalse()
    {
        var userBefore = this.sampleDataFactory.BuildUser() with { PasswordHash = "current-hash" };

        using (var dbContext = this.dbFixture.CreateDbContext())
        {
            dbContext.Users.Add(userBefore);
            await dbContext.SaveChangesAsync();
        }

        using (var dbContext = this.dbFixture.CreateDbContext())
        {
            Assert.False(await this.queries.SaveUpgradedPasswordHash(
                dbContext, this.sampleDataFactory.NextInt(), "current-hash", "new-hash"));
        }

        using (var dbContext = this.dbFixture.CreateDbContext())
        {
            var userAfter = await dbContext.Users.FindAsync(userBefore.Id);
            Assert.Equal(userBefore, userAfter);
        }
    }

    [Fact]
    public async Task SaveUpgradedPasswordHash_WhenCurrentHashDoesNotMatch_DoesNotUpdateHashAndReturnsFalse()
    {
        var userBefore = this.sampleDataFactory.BuildUser();

        using (var dbContext = this.dbFixture.CreateDbContext())
        {
            dbContext.Users.Add(userBefore);
            await dbContext.SaveChangesAsync();
        }

        using (var dbContext = this.dbFixture.CreateDbContext())
        {
            Assert.False(await this.queries.SaveUpgradedPasswordHash(
                dbContext, userBefore.Id, "stale-hash", "new-hash"));
        }

        using (var dbContext = this.dbFixture.CreateDbContext())
        {
            var userAfter = await dbContext.Users.FindAsync(userBefore.Id);
            Assert.Equal(userBefore, userAfter);
        }
    }

    #endregion
}
