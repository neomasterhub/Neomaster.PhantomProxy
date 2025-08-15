using System.Security.Cryptography;

namespace Neomaster.PhantomProxy.Common;

/// <summary>
/// Utility class for RSA encryption and decryption operations.
/// </summary>
public static class RsaEncryptor
{
  public static byte[] Decrypt(byte[] encrypted, string pem, RSAEncryptionPadding? encryptionPadding = null)
  {
    using var rsa = RSA.Create();
    rsa.ImportFromPem(pem);
    var decrypted = rsa.Decrypt(encrypted, encryptionPadding ?? RSAEncryptionPadding.OaepSHA256);

    return decrypted;
  }

  public static byte[] Encrypt(byte[] data, string pem, RSAEncryptionPadding? encryptionPadding = null)
  {
    using var rsa = RSA.Create();
    rsa.ImportFromPem(pem);
    var encrypted = rsa.Encrypt(data, encryptionPadding ?? RSAEncryptionPadding.OaepSHA256);

    return encrypted;
  }

  public static RsaPems GeneratePems(int keySizeInBits = 4096)
  {
    using var rsa = RSA.Create(keySizeInBits);

    var pems = new RsaPems
    {
      PublicPem = rsa.ExportSubjectPublicKeyInfoPem(),
      PrivatePem = rsa.ExportPkcs8PrivateKeyPem(),
    };

    return pems;
  }
}
