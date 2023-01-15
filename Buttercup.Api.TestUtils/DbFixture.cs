using System.Security.Cryptography;
using System.Text;
using Buttercup.Api.DbModel;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Xunit;

namespace Buttercup.Api.TestUtils;

/// <summary>
/// Represents a fixture that can be used to test components using a real database.
/// </summary>
/// <remarks>
/// <para>
/// As well as creating and destroying the test database, this fixture acts as an <see
/// cref="IDbContextFactory{T}" /> that can be injected into components to provide an <see
/// cref="AppDbContext" /> that connects to the test database.
/// </para>
/// <para>
/// Type parameter <typeparamref name="T" /> is used to generate a name for the test database.
/// Collections and classes that use this fixture with a distinct type for <typeparamref name="T" />
/// can safely run in parallel.
/// </para>
/// </remarks>
/// <typeparam name="T">
/// The collection or test class
/// </typeparam>
public class DbFixture<T> : IAsyncLifetime, IDbContextFactory<AppDbContext>
{
    public DbFixture()
    {
        this.DatabaseName = $"buttercup_test_{ComputeDatabaseNameSuffix()}";

        this.ConnectionString = new NpgsqlConnectionStringBuilder()
        {
            Host = "localhost",
            Username = "buttercup_dev",
            Password = "buttercup_dev",
            Database = this.DatabaseName
        }.ToString();
    }

    /// <summary>
    /// Gets the connection string for the test database.
    /// </summary>
    public string ConnectionString { get; }

    /// <summary>
    /// Gets the name of the test database.
    /// </summary>
    public string DatabaseName { get; }

    /// <summary>
    /// Creates a new <see cref="AppDbContext" /> that connects to the test database.
    /// </summary>
    /// <returns>
    /// A new <see cref="AppDbContext" />.
    /// </returns>
    public AppDbContext CreateDbContext() => new(
        new DbContextOptionsBuilder().UseAppDbOptions(this.ConnectionString).Options);

    private async Task RecreateDatabase()
    {
        using var dbContext = this.CreateDbContext();

        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
    }

    private async Task DeleteDatabase()
    {
        using var dbContext = this.CreateDbContext();

        await dbContext.Database.EnsureDeletedAsync();
    }

    private static string ComputeDatabaseNameSuffix()
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(typeof(T).AssemblyQualifiedName!));
        return Convert.ToHexString(hash)[..10];
    }

    Task IAsyncLifetime.InitializeAsync() => this.RecreateDatabase();

    Task IAsyncLifetime.DisposeAsync() => this.DeleteDatabase();
}
