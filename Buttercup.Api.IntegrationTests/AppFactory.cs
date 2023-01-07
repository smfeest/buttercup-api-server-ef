using Buttercup.Api.DbModel;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Buttercup.Api.IntegrationTests;

public class AppFactory : WebApplicationFactory<Query>, IAsyncLifetime
{
    Task IAsyncLifetime.InitializeAsync() => this.RecreateDatabase();

    async Task IAsyncLifetime.DisposeAsync() => await this.DisposeAsync();

    public async Task<AppDbContext> CreateAppDbContext()
    {
        using var scope = this.Services.CreateScope();

        var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();

        return await contextFactory.CreateDbContextAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder) =>
        builder
            .UseSetting(
                "ConnectionStrings:AppDb",
                "Host=localhost;Username=buttercup_dev;Password=buttercup_dev;Database=buttercup_test")
            .UseSetting("HostBuilder:ReloadConfigOnChange", bool.FalseString);

    private async Task RecreateDatabase()
    {
        using var context = await this.CreateAppDbContext();

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
}
