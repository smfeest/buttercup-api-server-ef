using Microsoft.EntityFrameworkCore;

namespace Buttercup.Api.DbModel;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    { }

    public DbSet<Recipe> Recipes => Set<Recipe>();

    public DbSet<User> Users => Set<User>();

    public DbSet<Session> Sessions => Set<Session>();
}
