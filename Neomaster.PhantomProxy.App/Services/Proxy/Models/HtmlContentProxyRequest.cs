namespace Neomaster.PhantomProxy.App;

/// <summary>
/// Proxy request for HTML content.
/// </summary>
public record HtmlContentProxyRequest
{
  /// <summary>
  /// Target URL to fetch.
  /// </summary>
  public string Url { get; init; } = string.Empty;
}
