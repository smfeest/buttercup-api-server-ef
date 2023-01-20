using Buttercup.Api.TestUtils;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Buttercup.Api.Security;

public sealed class PasswordAuthenticationQueriesTests :
    IClassFixture<DbFixture<PasswordAuthenticationQueriesTests>>, IAsyncLifetime
{
    private readonly DbFixture<PasswordAuthenticationQueriesTests> dbFixture;
    private readonly PasswordAuthenticationQueries passwordAuthenticationQueries = new();
    private readonly SampleDataFactory sampleDataFactory = new();

    public PasswordAuthenticationQueriesTests(
        DbFixture<PasswordAuthenticationQueriesTests> databaseFixture) =>
        this.dbFixture = databaseFixture;

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        using var dbContext = this.dbFixture.CreateDbContext();
        await dbContext.Users.ExecuteDeleteAsync();
    }

    #region FindUserByEmail

    [Fact]
    public async Task FindUserByEmail_ReturnsSpecifiedUser()
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
                await this.passwordAuthenticationQueries.FindUserByEmail(dbContext, user.Email));
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
        var userBefore = this.sampleDataFactory.BuildUser() with { PasswordHash = "current-hash" };

        using (var dbContext = this.dbFixture.CreateDbContext())
        {
            dbContext.Users.Add(userBefore);
            await dbContext.SaveChangesAsync();
        }

        using (var dbContext = this.dbFixture.CreateDbContext())
        {
            Assert.True(await this.passwordAuthenticationQueries.SaveUpgradedPasswordHash(
                dbContext, userBefore.Id, "current-hash", "new-hash"));
        }

        using (var dbContext = this.dbFixture.CreateDbContext())
        {
            var userAfter = await dbContext.Users.FindAsync(userBefore.Id);
            Assert.Equal(userBefore with { PasswordHash = "new-hash" }, userAfter);
        }
    }

    [Fact]
    public async Task SaveUpgradedPasswordHash_DoesNotUpdateHashAndReturnsFalseWhenUserIdDoesNotMatch()
    {
        var userBefore = this.sampleDataFactory.BuildUser() with { PasswordHash = "current-hash" };

        using (var dbContext = this.dbFixture.CreateDbContext())
        {
            dbContext.Users.Add(userBefore);
            await dbContext.SaveChangesAsync();
        }

        using (var dbContext = this.dbFixture.CreateDbContext())
        {
            Assert.False(await this.passwordAuthenticationQueries.SaveUpgradedPasswordHash(
                dbContext, this.sampleDataFactory.NextInt(), "current-hash", "new-hash"));
        }

        using (var dbContext = this.dbFixture.CreateDbContext())
        {
            var userAfter = await dbContext.Users.FindAsync(userBefore.Id);
            Assert.Equal(userBefore, userAfter);
        }
    }

    [Fact]
    public async Task SaveUpgradedPasswordHash_DoesNotUpdateHashAndReturnsFalseWhenCurrentHashDoesNotMatch()
    {
        var userBefore = this.sampleDataFactory.BuildUser();

        using (var dbContext = this.dbFixture.CreateDbContext())
        {
            dbContext.Users.Add(userBefore);
            await dbContext.SaveChangesAsync();
        }

        using (var dbContext = this.dbFixture.CreateDbContext())
        {
            Assert.False(await this.passwordAuthenticationQueries.SaveUpgradedPasswordHash(
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
