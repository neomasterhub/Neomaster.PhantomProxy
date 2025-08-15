namespace Neomaster.PhantomProxy.Infra;

/// <summary>
/// Settings for Phantom Proxy.
/// </summary>
public record PhantomProxySettings
{
  /// <summary>
  /// Value for <c>User-Agent</c> header.
  /// </summary>
  public string UserAgent { get; init; } = string.Empty;

  /// <summary>
  /// Value for <c>Referrer</c> header.
  /// </summary>
  public string Referrer { get; init; } = string.Empty;

  /// <summary>
  /// Names of attributes with URL values.
  /// </summary>
  public string[] UrlAttributeNames { get; init; } = [];

  /// <summary>
  /// Lifetime of encryption keys.
  /// </summary>
  public TimeSpan EncryptionKeysLifetime { get; init; }
}
