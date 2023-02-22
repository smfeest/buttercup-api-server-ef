using Buttercup.Api.DbModel;

namespace Buttercup.Api.Security;

/// <summary>
/// Defines the contract of the service used to manage sessions.
/// </summary>
public interface ISessionManager
{
    /// <summary>
    /// Creates a new session for a user.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>A task for the operation. The result is the new session.</returns>
    public Task<Session> CreateSession(User user);
}
