namespace Neomaster.PhantomProxy.App;

/// <summary>
/// Service for managing proxy operations.
/// </summary>
public interface IProxyService
{
  /// <summary>
  /// Proxies the given request and returns content.
  /// </summary>
  /// <param name="request">Proxy request with the target URL.</param>
  /// <returns>Proxy response.</returns>
  Task<ProxyResponse> ProxyRequestHtmlContentAsync(ProxyRequest request);

  /// <summary>
  /// Rewrites links in HTML document to proxy URLs.
  /// </summary>
  /// <param name="htmlDoc">HTML document.</param>
  /// <param name="baseUrl">Base URL for resolving relative links.</param>
  /// <param name="proxyUrlPrefix">Prefix of proxied URLs.</param>
  /// <returns>Rewritten HTML document with proxied links.</returns>
  string ProxyHtmlUrls(string htmlDoc, Uri baseUrl, string proxyUrlPrefix);
}
