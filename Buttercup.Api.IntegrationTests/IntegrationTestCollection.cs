using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace Buttercup.Api.IntegrationTests;

[CollectionDefinition(nameof(IntegrationTestCollection))]
[SuppressMessage(
    "Naming",
    "CA1711:IdentifiersShouldNotHaveIncorrectSuffix",
    Justification = "Represents an xUnit test collection")]
public class IntegrationTestCollection : ICollectionFixture<AppFactory>
{
}
