using System.Text.Json;
using Buttercup.Api.DbModel;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Buttercup.Api.IntegrationTests;

[Collection(nameof(IntegrationTestCollection))]
public class UsersTests
{
    private readonly AppFactory factory;

    public UsersTests(AppFactory factory) => this.factory = factory;

    [Fact]
    public async void QueryingUsers()
    {
        using var dbContext = await this.factory.CreateAppDbContext();

        try
        {
            var insertedUser = new SampleDataFactory().BuildUser();

            dbContext.Users.Add(insertedUser);

            await dbContext.SaveChangesAsync();

            using var client = this.factory.CreateClient();

            using var response = await client.PostAsJsonAsync(
                "/graphql", new { Query = "{ users { id name email timeZone created } }" });

            using var stream = await response.Content.ReadAsStreamAsync();

            using var document = await JsonDocument.ParseAsync(stream);

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
