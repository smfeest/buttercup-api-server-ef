using System.Text.Json;

namespace Buttercup.Api.IntegrationTests;

public static class JsonElementExtensions
{
    public static T? DeserializeObject<T>(this JsonElement element) =>
        element.Deserialize<T>(new JsonSerializerOptions(JsonSerializerDefaults.Web));

    public static IEnumerable<T?> DeserializeObjects<T>(this JsonElement arrayElement) =>
        arrayElement.EnumerateArray().Select(DeserializeObject<T>);
}
