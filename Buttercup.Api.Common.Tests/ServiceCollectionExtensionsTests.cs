using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Buttercup.Api;

public class ServiceCollectionExtensionsTests
{
    #region AddCommonServices

    [Fact]
    public void AddCommonServices_AddsClockAsTransientService() =>
        Assert.Contains(
            new ServiceCollection().AddCommonServices(),
            serviceDescriptor =>
                serviceDescriptor.ServiceType == typeof(IClock) &&
                serviceDescriptor.ImplementationType == typeof(Clock) &&
                serviceDescriptor.Lifetime == ServiceLifetime.Transient);

    #endregion
}
