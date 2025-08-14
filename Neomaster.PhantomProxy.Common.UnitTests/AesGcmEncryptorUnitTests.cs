using System.Security.Cryptography;

namespace Neomaster.PhantomProxy.Common.UnitTests;

public class AesGcmEncryptorUnitTests
{
  [Fact]
  public void Encrypt_ShouldReturnDifferentOutputThanInput()
  {
    var value = RandomNumberGenerator.GetBytes(100);
    var key = RandomNumberGenerator.GetBytes(32);
    var iv = RandomNumberGenerator.GetBytes(12);

    var encrypted = AesGcmEncryptor.Encrypt(value, key, iv);

    Assert.NotEqual(value, encrypted);
  }

  [Fact]
  public void Encrypt_ShouldReturnSameOutput_SameKeyAndSameIV()
  {
    var value = RandomNumberGenerator.GetBytes(100);
    var key = RandomNumberGenerator.GetBytes(32);
    var iv = RandomNumberGenerator.GetBytes(12);

    var encrypted1 = AesGcmEncryptor.Encrypt(value, key, iv);
    var encrypted2 = AesGcmEncryptor.Encrypt(value, key, iv);

    Assert.Equal(encrypted1, encrypted2);
  }

  [Fact]
  public void EncryptDecrypt_ShouldReturnOriginalValue()
  {
    var value = RandomNumberGenerator.GetBytes(100);
    var key = RandomNumberGenerator.GetBytes(32);
    var iv = RandomNumberGenerator.GetBytes(12);

    var encrypted = AesGcmEncryptor.Encrypt(value, key, iv);
    var decrypted = AesGcmEncryptor.Decrypt(encrypted, key, iv);

    Assert.Equal(value, decrypted);
  }

  [Fact]
  public void EncryptDecrypt_ShouldReturnEmpty()
  {
    var value = Array.Empty<byte>();
    var key = RandomNumberGenerator.GetBytes(32);
    var iv = RandomNumberGenerator.GetBytes(12);

    var encrypted = AesGcmEncryptor.Encrypt(value, key, iv);
    var decrypted = AesGcmEncryptor.Decrypt(encrypted, key, iv);

    Assert.Empty(decrypted);
  }

  [Fact]
  public void Decrypt_ShouldThrow_WrongKey()
  {
    var value = RandomNumberGenerator.GetBytes(100);
    var key = RandomNumberGenerator.GetBytes(32);
    var wrongKey = RandomNumberGenerator.GetBytes(32);
    var iv = RandomNumberGenerator.GetBytes(12);
    var encrypted = AesGcmEncryptor.Encrypt(value, key, iv);

    Assert.Throws<AuthenticationTagMismatchException>(() => AesGcmEncryptor.Decrypt(encrypted, wrongKey, iv));
  }

  [Fact]
  public void Decrypt_ShouldThrow_WrongIV()
  {
    var value = RandomNumberGenerator.GetBytes(100);
    var key = RandomNumberGenerator.GetBytes(32);
    var iv = RandomNumberGenerator.GetBytes(12);
    var wrongIV = RandomNumberGenerator.GetBytes(12);
    var encrypted = AesGcmEncryptor.Encrypt(value, key, iv);

    Assert.Throws<AuthenticationTagMismatchException>(() => AesGcmEncryptor.Decrypt(encrypted, key, wrongIV));
  }
}
