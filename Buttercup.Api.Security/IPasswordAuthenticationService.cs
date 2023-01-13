using Buttercup.Api.DbModel;

namespace Buttercup.Api.Security;

public interface IPasswordAuthenticationService
{
    /// <summary>
    /// Authenticates a user using an email address and password.
    /// </summary>
    /// <param name="email">
    /// The email address.
    /// </param>
    /// <param name="password">
    /// The password.
    /// </param>
    /// <returns>
    /// A task for the operation. The result is the user if successfully authenticated, or a null
    /// reference otherwise.
    /// </returns>
    Task<User?> Authenticate(string email, string password);
}
