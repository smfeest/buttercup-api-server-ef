using Buttercup.Api.DbModel;

namespace Buttercup.Api.Security;

public interface IPasswordAuthenticationQueries
{
    /// <summary>
    /// Finds a user by email address.
    /// </summary>
    /// <param name="dbContext">
    /// The database context.
    /// </param>
    /// <param name="email">
    /// The email address.
    /// </param>
    /// <returns>
    /// A task for the operation. The result is the user if a match is found, or a null reference
    /// otherwise.
    /// </returns>
    Task<User?> FindUserByEmail(AppDbContext dbContext, string email);

    /// <summary>
    /// Saves an upgraded password hash.
    /// </summary>
    /// <remarks>
    /// <para>
    /// To eliminate the risk of silently reverting a concurrent password change, the new hash is
    /// only saved if <paramref name="currentHash" /> matches the current hash for the user in the
    /// database.
    /// </para>
    /// </remarks>
    /// <param name="dbContext">
    /// The database context.
    /// </param>
    /// <param name="userId">
    /// The user ID.
    /// </param>
    /// <param name="currentHash">
    /// The current password hash.
    /// </param>
    /// <param name="newHash">
    /// The new password hash.
    /// </param>
    /// <returns>
    /// A task for the operation. The result is <c>true</c> if the password hash was updated;
    /// <b>false</b> if no user was found with the specified user ID and current password hash.
    /// </returns>
    Task<bool> SaveUpgradedPasswordHash(
        AppDbContext dbContext, long userId, string currentHash, string newHash);
}
