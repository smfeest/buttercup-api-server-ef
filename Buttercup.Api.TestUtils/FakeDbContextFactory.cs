using Buttercup.Api.DbModel;
using Microsoft.EntityFrameworkCore;

namespace Buttercup.Api.TestUtils;

public sealed class FakeDbContextFactory : IDbContextFactory<AppDbContext>, IDisposable
{
    public AppDbContext FakeDbContext { get; } = new();

    public AppDbContext CreateDbContext() => this.FakeDbContext;

    public void Dispose() => this.FakeDbContext.Dispose();
}
