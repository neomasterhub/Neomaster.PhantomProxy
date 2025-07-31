namespace Neomaster.PhantomProxy.Common;

/// <summary>
/// Represents single item in <c>srcset</c> value.
/// </summary>
public record SrcsetValueItem
{
  /// <summary>
  /// URL.
  /// </summary>
  public string Url { get; init; } = string.Empty;

  /// <summary>
  /// Image width (1w) or pixel density (2x) descriptor.
  /// </summary>
  public string Descriptor { get; init; } = string.Empty;
}
