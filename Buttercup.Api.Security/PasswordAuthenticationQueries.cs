using Buttercup.Api.DbModel;
using Microsoft.EntityFrameworkCore;

namespace Buttercup.Api.Security;

public sealed class PasswordAuthenticationQueries : IPasswordAuthenticationQueries
{
    public Task<User?> FindUserByEmail(AppDbContext dbContext, string email) =>
        dbContext.Users.SingleOrDefaultAsync(u => u.Email == email);

    public Task<bool> SaveUpgradedPasswordHash(
        AppDbContext dbContext, long userId, string currentHash, string newHash) =>
        throw new NotImplementedException();
}
