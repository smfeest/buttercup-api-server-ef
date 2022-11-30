using Microsoft.EntityFrameworkCore;

namespace Buttercup.Api.DbModel;

/// <summary>
/// Represents a session with the application database.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppDbContext" /> class.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    { }
}
