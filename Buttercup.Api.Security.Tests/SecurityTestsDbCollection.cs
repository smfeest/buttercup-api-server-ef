using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace Buttercup.Api.Security;

[CollectionDefinition(nameof(SecurityTestsDbCollection))]
[SuppressMessage(
    "Naming",
    "CA1711:IdentifiersShouldNotHaveIncorrectSuffix",
    Justification = "Represents an xUnit test collection")]
public sealed class SecurityTestsDbCollection : ICollectionFixture<SecurityTestsDbFixture>
{
}
