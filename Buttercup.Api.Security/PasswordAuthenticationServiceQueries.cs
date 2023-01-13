using Buttercup.Api.DbModel;
using Microsoft.EntityFrameworkCore;

namespace Buttercup.Api.Security;

/// <summary>
/// The default implementation of <see cref="IPasswordAuthenticationServiceQueries" />.
/// </summary>
public sealed class PasswordAuthenticationServiceQueries : IPasswordAuthenticationServiceQueries
{
    /// <inheritdoc/>
    public Task<User?> FindUserByEmail(AppDbContext dbContext, string email) =>
        dbContext.Users.SingleOrDefaultAsync(u => u.Email == email);

    /// <inheritdoc/>
    public async Task<bool> SaveUpgradedPasswordHash(
        AppDbContext dbContext, long userId, string currentHash, string newHash)
    {
        var affectedRows = await dbContext.Users
            .Where(u => u.Id == userId && u.PasswordHash == currentHash)
            .ExecuteUpdateAsync(s => s.SetProperty(s => s.PasswordHash, newHash));

        return affectedRows > 0;
    }
}
