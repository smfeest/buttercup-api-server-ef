using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.WebUtilities;

namespace Buttercup.Api.Security;

/// <summary>
/// The default implementation of <see cref="ISessionTokenEncoder" />.
/// </summary>
public sealed class SessionTokenEncoder : ISessionTokenEncoder
{
    private readonly IDataProtector dataProtector;

    /// <summary>
    /// Initializes a new instance of the the <see cref="SessionTokenEncoder" /> class.
    /// </summary>
    /// <param name="dataProtectionProvider">The data protection provider.</param>
    public SessionTokenEncoder(IDataProtectionProvider dataProtectionProvider) =>
        this.dataProtector = dataProtectionProvider.CreateProtector(nameof(SessionTokenEncoder));

    /// <inheritdoc/>
    public string Encode(SessionTokenPayload payload)
    {
        var payloadData = SerializePayload(payload);
        var encryptedPayloadData = this.dataProtector.Protect(payloadData);

        return WebEncoders.Base64UrlEncode(encryptedPayloadData);
    }

    /// <inheritdoc/>
    public SessionTokenPayload Decode(string token)
    {
        var encryptedPayloadData = WebEncoders.Base64UrlDecode(token);
        var payloadData = this.dataProtector.Unprotect(encryptedPayloadData);

        return DeserializePayload(payloadData);
    }

    private static SessionTokenPayload DeserializePayload(byte[] data)
    {
        using var stream = new MemoryStream(data);
        using var reader = new BinaryReader(stream);

        return new(
            reader.ReadInt64(), reader.ReadInt32(), (SessionTokenType)reader.ReadInt32());
    }

    private static byte[] SerializePayload(SessionTokenPayload payload)
    {
        using var stream = new MemoryStream();

        using (var writer = new BinaryWriter(stream))
        {
            writer.Write(payload.SessionId);
            writer.Write(payload.Generation);
            writer.Write((int)payload.TokenType);
        }

        return stream.ToArray();
    }
}
