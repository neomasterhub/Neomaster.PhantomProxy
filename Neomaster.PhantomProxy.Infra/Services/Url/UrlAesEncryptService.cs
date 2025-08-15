using System.Text;
using Neomaster.PhantomProxy.App;
using Neomaster.PhantomProxy.Common;

namespace Neomaster.PhantomProxy.Infra;

/// <inheritdoc/>
public class UrlAesEncryptService : IUrlEncryptService
{
  /// <inheritdoc/>
  public string Encrypt(string url, EncryptionOptions? encryptionOptions = null)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(url);
    ArgumentNullException.ThrowIfNull(encryptionOptions);

    if (encryptionOptions.AesKey.Length == 0)
    {
      throw new ArgumentException("Encryption options must contain AES key.");
    }

    if (encryptionOptions.AesIV.Length == 0)
    {
      throw new ArgumentException("Encryption options must contain AES IV.");
    }

    var urlBytes = Encoding.UTF8.GetBytes(url);
    var urlBytesEncrypted = AesGcmEncryptor.Encrypt(urlBytes, encryptionOptions.AesKey, encryptionOptions.AesIV);
    var urlBytesEncryptedBase64Escaped = Uri.EscapeDataString(Convert.ToBase64String(urlBytesEncrypted));

    return urlBytesEncryptedBase64Escaped;
  }

  /// <inheritdoc/>
  public string Decrypt(string encryptedUrl, EncryptionOptions? encryptionOptions = null)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(encryptedUrl);
    ArgumentNullException.ThrowIfNull(encryptionOptions);

    if (encryptionOptions.AesKey.Length == 0)
    {
      throw new ArgumentException("Encryption options must contain AES key.");
    }

    if (encryptionOptions.AesIV.Length == 0)
    {
      throw new ArgumentException("Encryption options must contain AES IV.");
    }

    var encryptedUrlBytes = Convert.FromBase64String(Uri.UnescapeDataString(encryptedUrl));
    var urlBytes = AesGcmEncryptor.Decrypt(encryptedUrlBytes, encryptionOptions.AesKey, encryptionOptions.AesIV);
    var url = Encoding.UTF8.GetString(urlBytes);

    return url;
  }
}
