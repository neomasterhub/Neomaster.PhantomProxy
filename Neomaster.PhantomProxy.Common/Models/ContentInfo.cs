namespace Neomaster.PhantomProxy.Common;

/// <summary>
/// Represents content information from HTTP response.
/// </summary>
public record ContentInfo
{
  /// <summary>
  /// Media type from Content-Type header ("text/html").
  /// </summary>
  public string MediaType { get; init; } = string.Empty;

  /// <summary>
  /// Charset from Content-Type header ("utf-8").
  /// </summary>
  public string Charset { get; init; } = string.Empty;
}
