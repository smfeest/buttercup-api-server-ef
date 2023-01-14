using Buttercup.Api.DbModel;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Buttercup.Api.TestUtils;

public class TestDbFixture : IAsyncLifetime
{
    public TestDbFixture(string databaseName) =>
        this.DbContextFactory = new TestDbContextFactory(databaseName);

    public IDbContextFactory<AppDbContext> DbContextFactory { get; }

    public Task InitializeAsync() => this.RecreateDatabase();

    public Task DisposeAsync() => Task.CompletedTask;

    private async Task RecreateDatabase()
    {
        using var dbContext = this.DbContextFactory.CreateDbContext();

        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
    }
}
