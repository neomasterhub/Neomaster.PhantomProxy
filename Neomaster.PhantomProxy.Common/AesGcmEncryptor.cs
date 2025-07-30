using System.Security.Cryptography;

namespace Neomaster.PhantomProxy.Common;

/// <summary>
/// Utility class for AES-GCM encryption and decryption operations.
/// </summary>
public static class AesGcmEncryptor
{
  public static byte[] Decrypt(byte[] encrypted, string password)
  {
    var iv = encrypted[..12];
    var cipherTextWithTag = encrypted[12..];
    var cipherText = cipherTextWithTag[..^16];
    var tag = cipherTextWithTag[^16..];
    var decrypted = new byte[cipherText.Length];

    using var pbkdf2 = new Rfc2898DeriveBytes(password, [], 1, HashAlgorithmName.SHA256);
    var key = pbkdf2.GetBytes(32);

    using var aes = new AesGcm(key);
    aes.Decrypt(iv, cipherText, tag, decrypted);

    return decrypted;
  }

  public static byte[] Encrypt(byte[] value, string password)
  {
    var iv = new byte[12];
    RandomNumberGenerator.Fill(iv);

    using var pbkdf2 = new Rfc2898DeriveBytes(password, [], 1, HashAlgorithmName.SHA256);
    var key = pbkdf2.GetBytes(32);

    var cipherBytes = new byte[value.Length];
    var tag = new byte[16];

    using (var aes = new AesGcm(key))
    {
      aes.Encrypt(iv, value, cipherBytes, tag);
    }

    var combined = new byte[iv.Length + cipherBytes.Length + tag.Length];
    Buffer.BlockCopy(iv, 0, combined, 0, iv.Length);
    Buffer.BlockCopy(cipherBytes, 0, combined, iv.Length, cipherBytes.Length);
    Buffer.BlockCopy(tag, 0, combined, iv.Length + cipherBytes.Length, tag.Length);

    return combined;
  }
}
