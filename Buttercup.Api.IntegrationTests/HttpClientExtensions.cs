using System.Text.Json;

namespace Buttercup.Api.IntegrationTests;

public static class HttpClientExtensions
{
    public static async Task<JsonDocument> PostQuery(
        this HttpClient client, string query, object? variables = null)
    {
        using var response = await client.PostAsJsonAsync(
            "/graphql", new { Query = query, Variables = variables ?? new() });

        using var stream = await response.Content.ReadAsStreamAsync();

        return await JsonDocument.ParseAsync(stream);
    }
}
