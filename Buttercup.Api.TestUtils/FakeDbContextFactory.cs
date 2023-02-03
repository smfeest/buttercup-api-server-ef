using Buttercup.Api.DbModel;
using Microsoft.EntityFrameworkCore;

namespace Buttercup.Api.TestUtils;

/// <summary>
/// A fake implementation of <see cref="IDbContextFactory{TContext}" /> that always returns an
/// unconfigured, singleton instance of <see cref="AppDbContext" />.
/// </summary>
/// <remarks>
/// This fake is intended to be used in tests where all the queries that would usually be executed
/// using the <see cref="AppDbContext" /> have been stubbed out through a mock query provider. Tests
/// that require a real database connection should typically use <see cref="DbFixture{T}" />
/// instead.
/// </remarks>
public sealed class FakeDbContextFactory : IDbContextFactory<AppDbContext>, IDisposable
{
    /// <summary>
    /// Gets the unconfigured, singleton instance of <see cref="AppDbContext" /> that's returned by
    /// <see cref="CreateDbContext()" />.
    /// </summary>
    public AppDbContext FakeDbContext { get; } = new();

    /// <inheritdoc/>
    public AppDbContext CreateDbContext() => this.FakeDbContext;

    /// <inheritdoc/>
    public void Dispose() => this.FakeDbContext.Dispose();
}
