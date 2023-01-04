using Microsoft.AspNetCore.Mvc.Testing;

namespace Buttercup.Api.IntegrationTests;

public class AppFactory : WebApplicationFactory<Query>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder) =>
        builder.UseSetting("HostBuilder:ReloadConfigOnChange", bool.FalseString);
}
