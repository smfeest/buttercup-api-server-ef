using Buttercup.Api.DbModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Buttercup.Api.Security;

public class ServiceCollectionExtensionsTests
{
    #region AddSecurityServices

    [Fact]
    public void AddSecurityServices_AddsPasswordAuthenticationServiceAsTransientService() =>
        Assert.Contains(
            new ServiceCollection().AddSecurityServices(),
            serviceDescriptor =>
                serviceDescriptor.ServiceType == typeof(IPasswordAuthenticationService) &&
                serviceDescriptor.ImplementationType == typeof(PasswordAuthenticationService) &&
                serviceDescriptor.Lifetime == ServiceLifetime.Transient);

    [Fact]
    public void AddSecurityServices_AddsPasswordAuthenticationServiceQueriesAsTransientService() =>
        Assert.Contains(
            new ServiceCollection().AddSecurityServices(),
            serviceDescriptor =>
                serviceDescriptor.ServiceType == typeof(IPasswordAuthenticationServiceQueries) &&
                serviceDescriptor.ImplementationType == typeof(PasswordAuthenticationServiceQueries) &&
                serviceDescriptor.Lifetime == ServiceLifetime.Transient);

    [Fact]
    public void AddSecurityServices_AddsPasswordHasherAsTransientService() =>
        Assert.Contains(
            new ServiceCollection().AddSecurityServices(),
            serviceDescriptor =>
                serviceDescriptor.ServiceType == typeof(IPasswordHasher<User>) &&
                serviceDescriptor.ImplementationType == typeof(PasswordHasher<User>) &&
                serviceDescriptor.Lifetime == ServiceLifetime.Transient);

    [Fact]
    public void AddSecurityServices_AddsSessionManagerAsTransientService() =>
        Assert.Contains(
            new ServiceCollection().AddSecurityServices(),
            serviceDescriptor =>
                serviceDescriptor.ServiceType == typeof(ISessionManager) &&
                serviceDescriptor.ImplementationType == typeof(SessionManager) &&
                serviceDescriptor.Lifetime == ServiceLifetime.Transient);

    [Fact]
    public void AddSecurityServices_AddsSessionTokenEncoderAsTransientService() =>
        Assert.Contains(
            new ServiceCollection().AddSecurityServices(),
            serviceDescriptor =>
                serviceDescriptor.ServiceType == typeof(ISessionTokenEncoder) &&
                serviceDescriptor.ImplementationType == typeof(SessionTokenEncoder) &&
                serviceDescriptor.Lifetime == ServiceLifetime.Transient);

    #endregion
}
