namespace Buttercup.Api.Security;

/// <summary>
/// Represents the payload of an access or refresh token associated with a session.
/// </summary>
/// <param name="SessionId">The session ID.</param>
/// <param name="Generation">The token generation.</param>
/// <param name="TokenType">The token type.</param>
public sealed record SessionTokenPayload(
    long SessionId, int Generation, SessionTokenType TokenType);
