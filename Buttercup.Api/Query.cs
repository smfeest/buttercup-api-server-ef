using Buttercup.Api.DbModel;

namespace Buttercup.Api;

public sealed class Query
{
    public IQueryable<User> GetUsers(AppDbContext dbContext) => dbContext.Users;
}
