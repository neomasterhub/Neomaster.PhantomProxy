using HtmlAgilityPack;

namespace Neomaster.PhantomProxy.App;

/// <summary>
/// Service for managing proxy operations.
/// </summary>
public interface IProxyService
{
  /// <summary>
  /// Proxies the given request and returns HTML content.
  /// </summary>
  /// <param name="request">Proxy request with the target URL.</param>
  /// <returns>Proxy response containing HTML content.</returns>
  Task<HtmlContentProxyResponse> ProxyRequestHtmlContentAsync(HtmlContentProxyRequest request);

  /// <summary>
  /// Rewrites links in HTML document to proxy URLs.
  /// </summary>
  /// <param name="htmlDoc">HTML document.</param>
  /// <param name="baseUrl">Base URL for resolving relative links.</param>
  /// <param name="proxyUrlPrefix">Prefix of proxied URLs.</param>
  /// <returns>Rewritten HTML document with proxied links.</returns>
  string RewriteLinksWithProxyUrls(string htmlDoc, Uri baseUrl, string proxyUrlPrefix);

  /// <summary>
  /// Rewrites attribute values in HTML document to proxy URLs.
  /// </summary>
  /// <param name="doc">HTML document.</param>
  /// <param name="baseUri">Base URI used to resolve relative links.</param>
  /// <param name="proxyUrlPrefix">Prefix of proxied URLs.</param>
  void RewriteAttributeValuesWithProxyUrls(HtmlDocument doc, Uri baseUri, string proxyUrlPrefix);
}
