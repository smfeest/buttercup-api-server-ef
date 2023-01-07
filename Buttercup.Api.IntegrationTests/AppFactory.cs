using Buttercup.Api.DbModel;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Buttercup.Api.IntegrationTests;

/// <summary>
/// The fixture used to bootstrap the application for integration testing.
/// </summary>
public class AppFactory : WebApplicationFactory<Query>, IAsyncLifetime
{
    Task IAsyncLifetime.InitializeAsync() => this.RecreateDatabase();

    async Task IAsyncLifetime.DisposeAsync() => await this.DisposeAsync();

    /// <summary>
    /// Creates a new instance of <see cref="AppDbContext" /> that connects to the test database.
    /// </summary>
    /// <returns>The new <see cref="AppDbContext" /> instance.</returns>
    public async Task<AppDbContext> CreateAppDbContext()
    {
        using var scope = this.Services.CreateScope();

        var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();

        return await contextFactory.CreateDbContextAsync();
    }

    /// <inheritdoc/>
    protected override void ConfigureWebHost(IWebHostBuilder builder) =>
        builder
            .UseSetting(
                "ConnectionStrings:AppDb",
                "Host=localhost;Username=buttercup_dev;Password=buttercup_dev;Database=buttercup_test")
            .UseSetting("HostBuilder:ReloadConfigOnChange", bool.FalseString)
            .UseSetting("Logging:LogLevel:Default", "Warning");

    private async Task RecreateDatabase()
    {
        using var context = await this.CreateAppDbContext();

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
}
