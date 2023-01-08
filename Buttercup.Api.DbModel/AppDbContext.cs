using Microsoft.EntityFrameworkCore;

namespace Buttercup.Api.DbModel;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    { }

    public DbSet<Recipe> Recipes => Set<Recipe>();

    public DbSet<User> Users => Set<User>();
}
