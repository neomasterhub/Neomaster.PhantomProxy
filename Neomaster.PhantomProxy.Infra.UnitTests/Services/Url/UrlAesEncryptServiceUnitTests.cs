using System.Security.Cryptography;
using Neomaster.PhantomProxy.App;

namespace Neomaster.PhantomProxy.Infra.UnitTests;

public class UrlAesEncryptServiceUnitTests
{
  private static readonly UrlAesEncryptService _sut = new();

  [Fact]
  public void EncryptDecrypt_ShouldReturnOriginalUrl_ValidInputs()
  {
    const string url = "1";
    var encryptionOptions = new EncryptionOptions
    {
      AesKey = RandomNumberGenerator.GetBytes(32),
      AesIV = RandomNumberGenerator.GetBytes(12),
    };

    var encrypted = _sut.Encrypt(url, encryptionOptions);
    var decrypted = _sut.Decrypt(encrypted, encryptionOptions);

    Assert.Equal(url, decrypted);
    Assert.DoesNotContain("+", encrypted);
    Assert.DoesNotContain("/", encrypted);
    Assert.DoesNotContain("=", encrypted);
  }
}
