using Buttercup.Api.DbModel;

namespace Buttercup.Api;

public sealed class Query
{
    [UseSingleOrDefault]
    [UseProjection]
    public IQueryable<User> GetUser(AppDbContext dbContext, long id) =>
        dbContext.Users.Where(u => u.Id == id);

    [UseProjection]
    public IQueryable<User> GetUsers(AppDbContext dbContext) => dbContext.Users;
}
