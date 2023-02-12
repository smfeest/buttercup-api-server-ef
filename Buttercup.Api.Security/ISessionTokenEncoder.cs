using System.Security.Cryptography;

namespace Buttercup.Api.Security;

/// <summary>
/// Defines the contract of the service used to encode and decode session tokens.
/// </summary>
public interface ISessionTokenEncoder
{
    /// <summary>
    /// Encodes a session token.
    /// </summary>
    /// <param name="payload">The token payload.</param>
    /// <returns>The encoded token.</returns>
    string Encode(SessionTokenPayload payload);

    /// <summary>
    /// Decodes a session token.
    /// </summary>
    /// <param name="token">The encoded token.</param>
    /// <returns>The token payload.</returns>
    /// <exception cref="FormatException">
    /// The encrypted token is not base64url encoded.
    /// </exception>
    /// <exception cref="CryptographicException">
    /// The token is malformed or encrypted with the wrong key.
    /// </exception>
    SessionTokenPayload Decode(string token);
}
