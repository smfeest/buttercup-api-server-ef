using System.Text.Json;
using Xunit;

namespace Buttercup.Api.IntegrationTests;

public static class JsonAssert
{
    public static void Equivalent<T>(T expected, JsonElement actual) =>
        Assert.Equivalent(expected, actual.DeserializeObject<T>());

    public static void Equivalent<T>(IEnumerable<T> expected, JsonElement actual) =>
        Assert.Equivalent(expected, actual.DeserializeObjects<T>());

    public static void Null(JsonElement actual) =>
        Assert.Equal(JsonValueKind.Null, actual.ValueKind);
}
