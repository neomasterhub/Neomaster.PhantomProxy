using System.Security.Cryptography;

namespace Neomaster.PhantomProxy.Common;

/// <summary>
/// Utility class for AES-GCM encryption and decryption operations.
/// </summary>
public static class AesGcmEncryptor
{
  public static byte[] Decrypt(byte[] encrypted, byte[] key, byte[] iv, int tagSizeInBytes = 16)
  {
    var tag = encrypted[^tagSizeInBytes..];
    var payload = encrypted[..^tagSizeInBytes];
    var decrypted = new byte[payload.Length];

    using var aes = new AesGcm(key, tagSizeInBytes);
    aes.Decrypt(iv, payload, tag, decrypted);

    return decrypted;
  }

  public static byte[] Encrypt(byte[] value, byte[] key, byte[] iv, int tagSizeInBytes = 16)
  {
    var tag = new byte[tagSizeInBytes];
    var payload = new byte[value.Length];

    using var aes = new AesGcm(key, tagSizeInBytes);
    aes.Encrypt(iv, value, payload, tag);

    var encrypted = new byte[payload.Length + tag.Length];
    Buffer.BlockCopy(payload, 0, encrypted, 0, payload.Length);
    Buffer.BlockCopy(tag, 0, encrypted, payload.Length, tag.Length);

    return encrypted;
  }
}
