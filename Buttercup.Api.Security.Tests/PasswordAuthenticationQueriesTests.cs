using Buttercup.Api.TestUtils;
using Xunit;

namespace Buttercup.Api.Security;

public sealed class PasswordAuthenticationQueriesTests :
    IClassFixture<DbFixture<PasswordAuthenticationQueriesTests>>
{
    [Fact]
    public void Test() => Assert.True(true);
}
