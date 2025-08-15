namespace Neomaster.PhantomProxy.App;

/// <summary>
/// Service for encrypting and decrypting URL.
/// </summary>
public interface IUrlEncryptService
{
  /// <summary>
  /// Encrypts given URL using provided encryption options.
  /// </summary>
  /// <param name="url">URL to encrypt.</param>
  /// <param name="encryptionOptions">Encryption options (e.g. keys, IV).</param>
  /// <returns>Encrypted URL.</returns>
  public string Encrypt(string url, EncryptionOptions? encryptionOptions = null);

  /// <summary>
  /// Decrypts given encrypted URL using provided encryption options.
  /// </summary>
  /// <param name="encryptedUrl">Encrypted URL.</param>
  /// <param name="encryptionOptions">Encryption options (e.g. keys, IV).</param>
  /// <returns>URL.</returns>
  public string Decrypt(string encryptedUrl, EncryptionOptions? encryptionOptions = null);
}
