using Buttercup.Api.DbModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Buttercup.Api.Security;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds security-related services to a service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection, so that calls can be chained.</returns>
    public static IServiceCollection AddSecurityServices(this IServiceCollection services) =>
        services
            .AddTransient<IPasswordAuthenticationService, PasswordAuthenticationService>()
            .AddTransient<IPasswordAuthenticationServiceQueries, PasswordAuthenticationServiceQueries>()
            .AddTransient<IPasswordHasher<User>, PasswordHasher<User>>()
            .AddTransient<ISessionManager, SessionManager>()
            .AddTransient<ISessionTokenEncoder, SessionTokenEncoder>();
}
