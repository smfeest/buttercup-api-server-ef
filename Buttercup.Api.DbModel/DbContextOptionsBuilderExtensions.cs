using Microsoft.EntityFrameworkCore;

namespace Buttercup.Api.DbModel;

public static class DbContextOptionsBuilderExtensions
{
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
