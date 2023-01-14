using Buttercup.Api.DbModel;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Buttercup.Api.TestUtils;

public sealed class TestDbContextFactory : IDbContextFactory<AppDbContext>
{
    private readonly string connectionString;

    public TestDbContextFactory(string databaseName) =>
        this.connectionString = new NpgsqlConnectionStringBuilder()
        {
            Host = "localhost",
            Username = "buttercup_dev",
            Password = "buttercup_dev",
            Database = databaseName
        }.ToString();

    public AppDbContext CreateDbContext() => new(
        new DbContextOptionsBuilder().UseAppDbOptions(this.connectionString).Options);
}
