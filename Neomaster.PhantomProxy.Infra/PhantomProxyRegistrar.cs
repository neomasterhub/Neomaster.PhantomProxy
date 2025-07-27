using Microsoft.Extensions.DependencyInjection;
using Neomaster.PhantomProxy.App;

namespace Neomaster.PhantomProxy.Infra;

/// <summary>
/// Registers Phantom Proxy dependencies.
/// </summary>
public static class PhantomProxyRegistrar
{
  /// <summary>
  /// Registers Phantom Proxy services and HTTP client.
  /// </summary>
  /// <param name="services">Service collection.</param>
  /// <returns>Updated service collection.</returns>
  public static IServiceCollection AddPhantomProxy(this IServiceCollection services)
  {
    services.AddHttpClient(nameof(PhantomProxy));
    services.AddScoped<IProxyService, ProxyService>();

    return services;
  }
}
