namespace Neomaster.PhantomProxy.App;

/// <summary>
/// Service for managing proxy operations.
/// </summary>
public interface IProxyService
{
  /// <summary>
  /// Proxies given request and returns content.
  /// </summary>
  /// <param name="request">Proxy request with target URL.</param>
  /// <returns>Proxy response.</returns>
  Task<ProxyResponse> ProxyRequestHtmlContentAsync(ProxyRequest request);

  /// <summary>
  /// Rewrites URLs inside CSS <c>@import</c> statements to proxy URLs.
  /// </summary>
  /// <param name="cssText">CSS text.</param>
  /// <param name="baseUri">Base URI for resolving relative URLs.</param>
  /// <param name="proxyUrlFormat">Proxy URL format string.</param>
  /// <param name="encryptionOptions">Encryption options (e.g. keys, IV).</param>
  /// <returns>Rewritten text with proxied URLs inside CSS <c>@import</c> statements.</returns>
  string ProxyCssImportUrls(string cssText, Uri baseUri, string proxyUrlFormat, EncryptionOptions? encryptionOptions = null);

  /// <summary>
  /// Rewrites URLs inside <c>url()</c> functions to proxy URLs.
  /// </summary>
  /// <param name="text">Text with <c>url()</c> functions.</param>
  /// <param name="baseUri">Base URI for resolving relative URLs.</param>
  /// <param name="proxyUrlFormat">Proxy URL format string.</param>
  /// <param name="encryptionOptions">Encryption options (e.g. keys, IV).</param>
  /// <returns>Rewritten text with proxied URLs inside <c>url()</c> functions.</returns>
  string ProxyUrlFunctionUrls(string text, Uri baseUri, string proxyUrlFormat, EncryptionOptions? encryptionOptions = null);

  /// <summary>
  /// Rewrites URLs in HTML document to proxy URLs.
  /// </summary>
  /// <param name="htmlDoc">HTML document.</param>
  /// <param name="baseUri">Base URI for resolving relative URLs.</param>
  /// <param name="proxyUrlFormat">Proxy URL format string.</param>
  /// <param name="encryptionOptions">Encryption options (e.g. keys, IV).</param>
  /// <returns>Rewritten HTML document with proxied URLs.</returns>
  string ProxyHtmlUrls(string htmlDoc, Uri baseUri, string proxyUrlFormat, EncryptionOptions? encryptionOptions = null);

  /// <summary>
  /// Returns proxied URL or original if invalid.
  /// </summary>
  /// <param name="url">URL to proxy.</param>
  /// <param name="baseUri">Base URI for resolving relative links.</param>
  /// <param name="proxyUrlFormat">Proxy URL format string.</param>
  /// <param name="encryptionOptions">Encryption options (e.g. keys, IV).</param>
  /// <returns>Proxied URL or original if invalid.</returns>
  string ProxyUrl(string url, Uri baseUri, string proxyUrlFormat, EncryptionOptions? encryptionOptions = null);
}
