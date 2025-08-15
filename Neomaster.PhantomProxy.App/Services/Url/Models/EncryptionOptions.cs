namespace Neomaster.PhantomProxy.App;

/// <summary>
/// Represents encryption options.
/// </summary>
public record EncryptionOptions
{
  public byte[] AesKey { get; init; } = [];
  public byte[] AesIV { get; init; } = [];
}
