namespace Neomaster.PhantomProxy.App;

/// <summary>
/// Proxy response with HTML content.
/// </summary>
public record HtmlContentProxyResponse
{
  /// <summary>
  /// HTML content returned from the target URL.
  /// </summary>
  public string RawHtml { get; init; } = string.Empty;
}
