using System.Text.Json;

namespace Buttercup.Api.IntegrationTests;

/// <summary>
/// Provides extension methods for <see cref="HttpClient" />.
/// </summary>
public static class HttpClientExtensions
{
    /// <summary>
    /// Posts a GraphQL query.
    /// </summary>
    /// <param name="client">The client used to send the request.</param>
    /// <param name="query">The query.</param>
    /// <param name="variables">The query variables.</param>
    /// <returns>
    /// A task for the operation. The result is a <see cref="JsonDocument" /> representing the
    /// result of the query.
    /// </returns>
    public static async Task<JsonDocument> PostQuery(
        this HttpClient client, string query, object? variables = null)
    {
        using var response = await client.PostAsJsonAsync(
            "/graphql", new { Query = query, Variables = variables ?? new() });

        using var stream = await response.Content.ReadAsStreamAsync();

        return await JsonDocument.ParseAsync(stream);
    }
}
