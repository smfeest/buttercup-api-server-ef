using Buttercup.Api.TestUtils;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Buttercup.Api.Security;

public sealed class PasswordAuthenticationQueriesTests :
    IClassFixture<DbFixture<PasswordAuthenticationQueriesTests>>
{
    private readonly DbFixture<PasswordAuthenticationQueriesTests> dbFixture;
    private readonly PasswordAuthenticationQueries passwordAuthenticationQueries = new();

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
            var sampleDataFactory = new SampleDataFactory();

            var users = new[]
            {
                sampleDataFactory.BuildUser(),
                sampleDataFactory.BuildUser(),
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
}
