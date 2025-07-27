namespace Neomaster.PhantomProxy.App;

/// <summary>
/// Proxy response with HTML content.
/// </summary>
public record HtmlContentProxyResponse
{
  /// <summary>
  /// HTML content bytes.
  /// </summary>
  public byte[] ContentBytes { get; init; } = [];

  /// <summary>
  /// HTML content type.
  /// </summary>
  public string ContentType { get; init; } = string.Empty;
}
