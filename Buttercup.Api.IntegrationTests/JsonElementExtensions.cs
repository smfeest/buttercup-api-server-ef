using System.Text.Json;

namespace Buttercup.Api.IntegrationTests;

/// <summary>
/// Provides extension methods for <see cref="JsonElement" />.
/// </summary>
public static class JsonElementExtensions
{
    /// <summary>
    /// Deserializes a JSON object as an object of type <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">The object type.</typeparam>
    /// <param name="element">
    /// The <see cref="JsonElement" /> representing the JSON object to be deserialized.
    /// </param>
    /// <returns>The deserialized object.</returns>
    public static T? DeserializeObject<T>(this JsonElement element) =>
        element.Deserialize<T>(new JsonSerializerOptions(JsonSerializerDefaults.Web));

    /// <summary>
    /// Deserializes a JSON array as an enumerable collection of objects of type <typeparamref
    /// name="T" />.
    /// </summary>
    /// <typeparam name="T">The object type.</typeparam>
    /// <param name="arrayElement">
    /// The <see cref="JsonElement" /> representing the JSON array to be deserialized.
    /// </param>
    /// <returns>The deserialized objects.</returns>
    public static IEnumerable<T?> DeserializeObjects<T>(this JsonElement arrayElement) =>
        arrayElement.EnumerateArray().Select(DeserializeObject<T>);
}
