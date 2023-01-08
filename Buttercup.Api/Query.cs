using Buttercup.Api.DbModel;

namespace Buttercup.Api;

/// <summary>
/// Represents the root `Query` type.
/// </summary>
public sealed class Query
{
    /// <summary>
    /// Returns an <see cref="IQueryable"/> for querying all users.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <returns>An <see cref="IQueryable"/> for querying all users.</returns>
    [UseProjection]
    public IQueryable<User> GetUsers(AppDbContext dbContext) => dbContext.Users;
}
