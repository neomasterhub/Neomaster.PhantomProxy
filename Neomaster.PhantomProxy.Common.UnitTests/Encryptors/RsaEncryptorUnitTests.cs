using System.Security.Cryptography;

namespace Neomaster.PhantomProxy.Common.UnitTests;

public class RsaEncryptorUnitTests
{
  [Fact]
  public void GeneratePems_ShouldReturnValidPems()
  {
    var pems = RsaEncryptor.GeneratePems();

    Assert.NotNull(pems);
    Assert.NotEmpty(pems.PublicPem);
    Assert.NotEmpty(pems.PrivatePem);
    Assert.NotEqual(pems.PublicPem, pems.PrivatePem);
    Assert.StartsWith("-----BEGIN PUBLIC KEY-----", pems.PublicPem);
    Assert.EndsWith("-----END PUBLIC KEY-----", pems.PublicPem);
    Assert.StartsWith("-----BEGIN PRIVATE KEY-----", pems.PrivatePem);
    Assert.EndsWith("-----END PRIVATE KEY-----", pems.PrivatePem);
  }

  [Fact]
  public void Encrypt_ShouldReturnDifferentOutputThanInput()
  {
    var value = RandomNumberGenerator.GetBytes(100);
    var pem = RsaEncryptor.GeneratePems().PublicPem;

    var encrypted = RsaEncryptor.Encrypt(value, pem);

    Assert.NotEqual(value, encrypted);
  }

  [Fact]
  public void Encrypt_ShouldReturnDifferentOutput_SamePem()
  {
    var value = RandomNumberGenerator.GetBytes(100);
    var pem = RsaEncryptor.GeneratePems().PublicPem;

    var encrypted1 = RsaEncryptor.Encrypt(value, pem);
    var encrypted2 = RsaEncryptor.Encrypt(value, pem);

    Assert.NotEqual(encrypted1, encrypted2);
  }

  [Fact]
  public void Encrypt_ShouldThrow_InvalidPem()
  {
    var value = RandomNumberGenerator.GetBytes(100);

    Assert.Throws<ArgumentException>(() => RsaEncryptor.Encrypt(value, "invalid pem"));
  }

  [Fact]
  public void EncryptDecrypt_ShouldReturnOriginalValue()
  {
    var value = RandomNumberGenerator.GetBytes(100);
    var pems = RsaEncryptor.GeneratePems();

    var encrypted = RsaEncryptor.Encrypt(value, pems.PublicPem);
    var decrypted = RsaEncryptor.Decrypt(encrypted, pems.PrivatePem);

    Assert.Equal(value, decrypted);
  }

  [Fact]
  public void Decrypt_ShouldThrow_InvalidPem()
  {
    var value = RandomNumberGenerator.GetBytes(100);
    var pem = RsaEncryptor.GeneratePems().PublicPem;
    var encrypted = RsaEncryptor.Encrypt(value, pem);

    Assert.Throws<ArgumentException>(() => RsaEncryptor.Decrypt(encrypted, "invalid pem"));
  }

  [Fact]
  public void Decrypt_ShouldThrow_UseEncryptionPem()
  {
    var value = RandomNumberGenerator.GetBytes(100);
    var pems = RsaEncryptor.GeneratePems();
    var encrypted = RsaEncryptor.Encrypt(value, pems.PublicPem);

    Assert.Throws<CryptographicException>(() => RsaEncryptor.Decrypt(encrypted, pems.PublicPem));
  }

  [Fact]
  public void Decrypt_ShouldThrow_WrongPem()
  {
    var value = RandomNumberGenerator.GetBytes(100);
    var pems1 = RsaEncryptor.GeneratePems();
    var pems2 = RsaEncryptor.GeneratePems();
    var encrypted = RsaEncryptor.Encrypt(value, pems1.PublicPem);

    Assert.Throws<CryptographicException>(() => RsaEncryptor.Decrypt(encrypted, pems2.PrivatePem));
  }
}
