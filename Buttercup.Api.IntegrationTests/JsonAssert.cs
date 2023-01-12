using System.Text.Json;
using Xunit;

namespace Buttercup.Api.IntegrationTests;

/// <summary>
/// Provides static methods that can be used to make assertions about JSON values.
/// </summary>
public static class JsonAssert
{
    /// <summary>
    /// Verifies that the deserialized value of a <see cref="JsonElement" /> is equivalent to a
    /// specified object of type <typeparamref name="T" />.
    /// </summary>
    /// <remarks>
    /// This method uses the same definition of equivalence as <see cref="Assert.Equivalent(object?,
    /// object?, bool)" />.
    /// </remarks>
    /// <typeparam name="T">The object type.</typeparam>
    /// <param name="expected">
    /// The object that the deserialized value is expected to be equivalent to.
    /// </param>
    /// <param name="actual">The <see cref="JsonElement" /> to be inspected.</param>
    /// <exception cref="Xunit.Sdk.EquivalentException">
    /// When the deserialized value of <paramref name="actual" /> is not equivalent to <paramref
    /// name="expected" />
    /// </exception>
    public static void Equivalent<T>(T expected, JsonElement actual) =>
        Assert.Equivalent(expected, actual.DeserializeObject<T>());

    /// <summary>
    /// Verifies that the deserialized value of a <see cref="JsonElement" /> is equivalent to a
    /// specified enumerable collection of objects of type <typeparamref name="T" />.
    /// </summary>
    /// <remarks>
    /// This method uses the same definition of equivalence as <see cref="Assert.Equivalent(object?,
    /// object?, bool)" />.
    /// </remarks>
    /// <typeparam name="T">The object type.</typeparam>
    /// <param name="expected">
    /// The collection of objects that the deserialized value is expected to be equivalent to.
    /// </param>
    /// <param name="actual">The <see cref="JsonElement" /> to be inspected.</param>
    /// <exception cref="Xunit.Sdk.EquivalentException">
    /// When the deserialized value of <paramref name="actual" /> is not equivalent to <paramref
    /// name="expected" />
    /// </exception>
    public static void Equivalent<T>(IEnumerable<T> expected, JsonElement actual) =>
        Assert.Equivalent(expected, actual.DeserializeObjects<T>());

    /// <summary>
    /// Verifies that the <see cref="JsonElement.ValueKind" /> of a <see cref="JsonElement" /> is
    /// <see cref="JsonValueKind.Null" />.
    /// </summary>
    /// <param name="element">The <see cref="JsonElement" /> to be inspected.</param>
    public static void Null(JsonElement element) =>
        Assert.Equal(JsonValueKind.Null, element.ValueKind);
}
