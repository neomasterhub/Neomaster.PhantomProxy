namespace Neomaster.PhantomProxy.Common;

/// <summary>
/// Represents pair of RSA PEM-encoded keys.
/// </summary>
public record RsaPems
{
  public string PublicPem { get; init; } = string.Empty;
  public string PrivatePem { get; init; } = string.Empty;
}
