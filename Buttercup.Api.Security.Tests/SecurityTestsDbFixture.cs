using Buttercup.Api.TestUtils;

namespace Buttercup.Api.Security;

public sealed class SecurityTestsDbFixture : TestDbFixture
{
    public SecurityTestsDbFixture() : base("buttercup_test_security")
    { }
}
