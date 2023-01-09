using Buttercup.Api.DbModel;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Buttercup.Api.IntegrationTests;

[Collection(nameof(IntegrationTestCollection))]
public class UsersTests
{
    private readonly AppFactory appFactory;
    private readonly SampleDataFactory sampleDataFactory = new();

    public UsersTests(AppFactory appFactory) => this.appFactory = appFactory;

    [Fact]
    public async void QueryingUser()
    {
        using var dbContext = await this.appFactory.CreateAppDbContext();

        try
        {
            var insertedUser = this.sampleDataFactory.BuildUser();

            dbContext.Users.Add(insertedUser);

            await dbContext.SaveChangesAsync();

            async Task<User?> QueryUser(long id)
            {
                using var client = this.appFactory.CreateClient();

                using var document = await client.PostQuery(
                    @"query($id: Long!) {
                        user(id: $id) {
                            id name email timeZone created
                        }
                    }",
                    new { id });

                return document.RootElement
                    .GetProperty("data")
                    .GetProperty("user")
                    .DeserializeObject<User>();
            }

            Assert.Equivalent(insertedUser, await QueryUser(insertedUser.Id));
            Assert.Null(await QueryUser(this.sampleDataFactory.NextInt()));
        }
        finally
        {
            await dbContext.Users.ExecuteDeleteAsync();
        }
    }

    [Fact]
    public async void QueryingUsers()
    {
        using var dbContext = await this.appFactory.CreateAppDbContext();

        try
        {
            var insertedUser = this.sampleDataFactory.BuildUser();

            dbContext.Users.Add(insertedUser);

            await dbContext.SaveChangesAsync();

            using var client = this.appFactory.CreateClient();

            using var document = await client.PostQuery(
                "{ users { id name email timeZone created } }");

            var returnedUsers = document.RootElement
                .GetProperty("data")
                .GetProperty("users")
                .DeserializeObjects<User>();

            Assert.Equivalent(insertedUser, Assert.Single(returnedUsers));
        }
        finally
        {
            await dbContext.Users.ExecuteDeleteAsync();
        }
    }
}
