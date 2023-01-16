using System.Text.Json;
using Buttercup.Api.TestUtils;
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
        using var dbContext = this.appFactory.DbContextFactory.CreateDbContext();

        try
        {
            var insertedUser = this.sampleDataFactory.BuildUser();

            dbContext.Users.Add(insertedUser);

            await dbContext.SaveChangesAsync();

            using var client = this.appFactory.CreateClient();

            Task<JsonDocument> PostUserQuery(long id) => client.PostQuery(
                @"query($id: Long!) {
                    user(id: $id) {
                        id name email timeZone created
                    }
                }",
                new { id });

            JsonElement GetUserElement(JsonDocument document) =>
                document.RootElement.GetProperty("data").GetProperty("user");

            using (var document = await PostUserQuery(insertedUser.Id))
            {
                JsonAssert.Equivalent(insertedUser, GetUserElement(document));
            }

            using (var document = await PostUserQuery(this.sampleDataFactory.NextInt()))
            {
                JsonAssert.Null(GetUserElement(document));
            }
        }
        finally
        {
            await dbContext.Users.ExecuteDeleteAsync();
        }
    }

    [Fact]
    public async void QueryingUsers()
    {
        using var dbContext = this.appFactory.DbContextFactory.CreateDbContext();

        try
        {
            var insertedUser = this.sampleDataFactory.BuildUser();

            dbContext.Users.Add(insertedUser);

            await dbContext.SaveChangesAsync();

            using var client = this.appFactory.CreateClient();

            using var document = await client.PostQuery(
                "{ users { id name email timeZone created } }");

            var returnedUsers = document.RootElement.GetProperty("data").GetProperty("users");

            JsonAssert.Equivalent(new[] { insertedUser }, returnedUsers);
        }
        finally
        {
            await dbContext.Users.ExecuteDeleteAsync();
        }
    }
}
