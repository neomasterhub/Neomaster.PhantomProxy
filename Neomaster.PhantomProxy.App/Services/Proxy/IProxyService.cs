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
}
