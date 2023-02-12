namespace Buttercup.Api.Security;

/// <summary>
/// Represents a session token type.
/// </summary>
public enum SessionTokenType
{
    /// <summary>
    /// The token is an access token.
    /// </summary>
    Access,

    /// <summary>
    /// The token is a refresh token.
    /// </summary>
    Refresh,
}
