using Microsoft.Extensions.DependencyInjection;

namespace Buttercup.Api;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds cross-cutting services to a service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection, so that calls can be chained.</returns>
    public static IServiceCollection AddCommonServices(this IServiceCollection services) =>
        services
            .AddTransient<IClock, Clock>();
}
