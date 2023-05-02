using Buttercup.Api.DbModel;
using Buttercup.Api.TestUtils;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Buttercup.Api.IntegrationTests;

/// <summary>
/// The fixture used to bootstrap the application for integration testing.
/// </summary>
public class AppFactory<TDatabaseName> : WebApplicationFactory<Query>, IAsyncLifetime
{
    private readonly DbFixture<TDatabaseName> dbFixture = new();

    /// <summary>
    /// Gets the database context factory.
    /// </summary>
    public IDbContextFactory<AppDbContext> DbContextFactory => this.dbFixture;

    /// <inheritdoc/>
    protected override void ConfigureWebHost(IWebHostBuilder builder) =>
        builder
            .UseSetting("ConnectionStrings:AppDb", dbFixture.ConnectionString)
            .UseSetting("HostBuilder:ReloadConfigOnChange", bool.FalseString)
            .UseSetting("Logging:LogLevel:Default", "Warning");

    Task IAsyncLifetime.InitializeAsync() => ((IAsyncLifetime)dbFixture).InitializeAsync();

    Task IAsyncLifetime.DisposeAsync() => ((IAsyncLifetime)dbFixture).DisposeAsync();
}
