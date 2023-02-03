using Microsoft.EntityFrameworkCore;

namespace Buttercup.Api.DbModel;

/// <summary>
/// Provides extension methods for <see cref="DbContextOptionsBuilder" />.
/// </summary>
public static class DbContextOptionsBuilderExtensions
{
    /// <summary>
    /// Adds the default options for the application database.
    /// </summary>
    /// <param name="options">The options builder.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <returns>The same options builder so that calls can be chained.</returns>
    public static DbContextOptionsBuilder UseAppDbOptions(
        this DbContextOptionsBuilder options, string? connectionString) =>
        options
            .UseNpgsql(
                connectionString,
                npgsqlOptions => npgsqlOptions
                    .MigrationsAssembly("Buttercup.Api.DbModel.Migrations")
                    .MigrationsHistoryTable("__migration_history")
                    .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .UseSnakeCaseNamingConvention();
}
