using Microsoft.AspNetCore.Mvc.Testing;

namespace Buttercup.Api.IntegrationTests;

/// <summary>
/// The fixture used to bootstrap the application for integration testing.
/// </summary>
public class AppFactory : WebApplicationFactory<Query>
{
    /// <inheritdoc/>
    protected override void ConfigureWebHost(IWebHostBuilder builder) =>
        builder.UseSetting("HostBuilder:ReloadConfigOnChange", bool.FalseString);
}
