using System.Text.Json;
using Xunit;

namespace Buttercup.Api.IntegrationTests;

[Collection(nameof(IntegrationTestCollection))]
public class PingTests
{
    private readonly AppFactory factory;

    public PingTests(AppFactory factory) => this.factory = factory;

    [Fact]
    public async void QueryingPing()
    {
        using var client = this.factory.CreateClient();

        using var response = await client.PostAsJsonAsync("/graphql", new { Query = "{ ping }" });

        using var stream = await response.Content.ReadAsStreamAsync();

        using var document = await JsonDocument.ParseAsync(stream);

        var pingValue = document
            .RootElement
            .GetProperty("data")
            .GetProperty("ping")
            .GetString();

        Assert.Equal("pong", pingValue);
    }
}
