using Buttercup.Api.DbModel;
using Buttercup.Api.TestUtils;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Buttercup.Api.IntegrationTests;

public class AppFactory : WebApplicationFactory<Query>, IAsyncLifetime
{
    private readonly DbFixture<AppFactory> dbFixture = new();

    public IDbContextFactory<AppDbContext> DbContextFactory => this.dbFixture;

    protected override void ConfigureWebHost(IWebHostBuilder builder) =>
        builder
            .UseSetting("ConnectionStrings:AppDb", dbFixture.ConnectionString)
            .UseSetting("HostBuilder:ReloadConfigOnChange", bool.FalseString)
            .UseSetting("Logging:LogLevel:Default", "Warning");

    Task IAsyncLifetime.InitializeAsync() => ((IAsyncLifetime)dbFixture).InitializeAsync();

    Task IAsyncLifetime.DisposeAsync() => ((IAsyncLifetime)dbFixture).DisposeAsync();
}
