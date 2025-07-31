namespace Neomaster.PhantomProxy.App;

/// <summary>
/// Proxy response.
/// </summary>
public record ProxyResponse
{
  /// <summary>
  /// Content bytes.
  /// </summary>
  public byte[] ContentBytes { get; init; } = [];

  /// <summary>
  /// Content type.
  /// </summary>
  public string ContentType { get; init; } = string.Empty;
}
