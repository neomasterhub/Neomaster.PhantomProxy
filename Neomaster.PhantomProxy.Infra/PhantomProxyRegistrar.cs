using Microsoft.Extensions.Configuration;
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
  /// <param name="configuration">Configuration settings.</param>
  /// <returns>Updated service collection.</returns>
  public static IServiceCollection AddPhantomProxy(this IServiceCollection services, IConfiguration configuration)
  {
    var settings = configuration.GetSection(nameof(PhantomProxySettings)).Get<PhantomProxySettings>();
    ArgumentNullException.ThrowIfNull(settings, nameof(settings));

    services.AddSingleton(settings);
    services.AddScoped<IProxyService, ProxyService>();
    services.AddHttpClient(nameof(PhantomProxy), client =>
    {
      client.DefaultRequestHeaders.UserAgent.ParseAdd(settings.UserAgent);
      client.DefaultRequestHeaders.Referrer = new Uri(settings.Referrer);
    });

    return services;
  }
}
