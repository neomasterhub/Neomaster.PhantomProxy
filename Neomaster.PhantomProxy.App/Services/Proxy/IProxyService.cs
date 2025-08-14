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
  /// <param name="baseUri">Base URI for resolving relative links.</param>
  /// <param name="proxyUrlFormat">Proxy URL format string.</param>
  /// <param name="aesKey">AES key.</param>
  /// <param name="aesIV">AES IV.</param>
  /// <returns>Rewritten HTML document with proxied links.</returns>
  string ProxyHtmlUrls(string htmlDoc, Uri baseUri, string proxyUrlFormat, byte[] aesKey, byte[] aesIV);

  /// <summary>
  /// Returns proxied URL or original if invalid.
  /// </summary>
  /// <param name="url">URL to proxy.</param>
  /// <param name="baseUri">Base URI for resolving relative links.</param>
  /// <param name="proxyUrlFormat">Proxy URL format string.</param>
  /// <param name="aesKey">AES key.</param>
  /// <param name="aesIV">AES IV.</param>
  /// <returns>Proxied URL or original if invalid.</returns>
  public string ProxyUrl(string url, Uri baseUri, string proxyUrlFormat, byte[] aesKey, byte[] aesIV);
}
