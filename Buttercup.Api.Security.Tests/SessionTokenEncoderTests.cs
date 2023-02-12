using Microsoft.AspNetCore.DataProtection;
using Xunit;

namespace Buttercup.Api.Security;

public sealed class SessionTokenEncoderTests
{
    [Theory]
    [InlineData(5, 10, SessionTokenType.Access)]
    [InlineData(15, 20, SessionTokenType.Refresh)]
    public void RoundTripsAllProperties(long sessionId, int generation, SessionTokenType tokenType)
    {
        var originalPayload = new SessionTokenPayload(sessionId, generation, tokenType);

        var encoder = new SessionTokenEncoder(new EphemeralDataProtectionProvider());

        var roundtrippedPayload = encoder.Decode(encoder.Encode(originalPayload));

        Assert.Equal(originalPayload, roundtrippedPayload);
    }
}
