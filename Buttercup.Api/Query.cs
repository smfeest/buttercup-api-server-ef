using Buttercup.Api.DbModel;

namespace Buttercup.Api;

/// <summary>
/// Represents the root `Query` type.
/// </summary>
public sealed class Query
{
    /// <summary>
    /// Returns an <see cref="IQueryable"/> for querying all recipes.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <returns>An <see cref="IQueryable"/> for querying all recipes.</returns>
    [UseProjection]
    public IQueryable<Recipe> GetRecipes(AppDbContext dbContext) => dbContext.Recipes;

    /// <summary>
    /// Returns an <see cref="IQueryable"/> for querying a specific user.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="id">The primary key of the user.</param>
    /// <returns>An <see cref="IQueryable"/> for querying the specified user.</returns>
    [UseSingleOrDefault]
    [UseProjection]
    public IQueryable<User> GetUser(AppDbContext dbContext, long id) =>
        dbContext.Users.Where(u => u.Id == id);

    /// <summary>
    /// Returns an <see cref="IQueryable"/> for querying all users.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <returns>An <see cref="IQueryable"/> for querying all users.</returns>
    [UseProjection]
    public IQueryable<User> GetUsers(AppDbContext dbContext) => dbContext.Users;
}
