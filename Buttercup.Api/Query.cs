using Buttercup.Api.DbModel;

namespace Buttercup.Api;

/// <summary>
/// Represents the root `Query` type.
/// </summary>
public sealed class Query
{
    /// <summary>
    /// Gets the string 'pong'.
    /// </summary>
    public string Ping => "pong";

    /// <summary>
    /// Returns an <see cref="IQueryable"/> for querying all users.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <returns>An <see cref="IQueryable"/> for querying all users.</returns>
    public IQueryable<User> GetUsers(AppDbContext dbContext) => dbContext.Users;
}
