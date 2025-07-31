namespace Neomaster.PhantomProxy.App;

/// <summary>
/// Proxy request.
/// </summary>
public record ProxyRequest
{
  /// <summary>
  /// Target URL to fetch.
  /// </summary>
  public string Url { get; init; } = string.Empty;
}
