using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace Buttercup.Api.IntegrationTests;

/// <summary>
/// The test collection that contains all integration tests.
/// </summary>
[CollectionDefinition(nameof(IntegrationTestCollection))]
[SuppressMessage(
    "Naming",
    "CA1711:IdentifiersShouldNotHaveIncorrectSuffix",
    Justification = "Represents an xUnit test collection")]
public class IntegrationTestCollection : ICollectionFixture<AppFactory>
{
}
