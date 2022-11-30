using Microsoft.EntityFrameworkCore;

namespace Buttercup.Api.DbModel;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    { }
}
