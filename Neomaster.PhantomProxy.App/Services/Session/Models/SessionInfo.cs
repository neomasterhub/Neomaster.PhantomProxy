namespace Neomaster.PhantomProxy.App;

/// <summary>
/// Represents session information.
/// </summary>
public record SessionInfo
{
  /// <summary>
  /// Public PEM-encoded key.
  /// </summary>
  public string Pem { get; init; } = string.Empty;

  /// <summary>
  /// Session lifetime.
  /// </summary>
  public TimeSpan Lifetime { get; init; }

  /// <summary>
  /// Session lifetime in milliseconds.
  /// </summary>
  public int LifetimeMs => (int)Lifetime.TotalMilliseconds;
}
