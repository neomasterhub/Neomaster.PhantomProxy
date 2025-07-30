using System.Text;

namespace Neomaster.PhantomProxy.Common.UnitTests;

public class AesGcmEncryptorUnitTests
{
  [Theory]
  [InlineData("")]
  [InlineData("1")]
  public void Encrypt_ShouldNotReturnInput(string inputText)
  {
    var input = Encoding.UTF8.GetBytes(inputText);

    var encrypted = AesGcmEncryptor.Encrypt(input, "1");

    Assert.False(encrypted.SequenceEqual(input));
  }

  [Fact]
  public void Encrypt_ShouldReturnDifferentEncryptedValues()
  {
    var inputPasswordPairs = new List<string[]>
      {
        new string[] { "1", "1" },
        new string[] { "1", "1" },
        new string[] { "2", "1" },
        new string[] { "2", "2" },
      }
      .Select(ip => new
      {
        Input = Encoding.UTF8.GetBytes(ip[0]),
        Password = ip[1],
      })
      .ToArray();

    var encryptedValuesCount = inputPasswordPairs
      .Select(ip => AesGcmEncryptor.Encrypt(ip.Input, ip.Password))
      .DistinctBy(Convert.ToBase64String)
      .Count();

    Assert.Equal(inputPasswordPairs.Length, encryptedValuesCount);
  }

  [Fact]
  public void Decrypt_ShouldReturnSourceValue()
  {
    var inputPasswordPairs = new List<string[]>
      {
        new string[] { "1", "1" },
        new string[] { "1", "1" },
        new string[] { "2", "1" },
        new string[] { "2", "2" },
      }
      .Select(ip => new
      {
        Input = Encoding.UTF8.GetBytes(ip[0]),
        Password = ip[1],
      })
      .ToArray();
    var encryptedValues = inputPasswordPairs
      .Select(ip => new
      {
        Encrypted = AesGcmEncryptor.Encrypt(ip.Input, ip.Password),
        ip.Password,
      })
      .ToArray();

    var decryptedValues = encryptedValues
      .Select(e => AesGcmEncryptor.Decrypt(e.Encrypted, e.Password))
      .ToArray();

    Assert.Equal(encryptedValues.Length, decryptedValues.Length);
    Assert.All(decryptedValues, (de, i) =>
    {
      Assert.True(de.SequenceEqual(inputPasswordPairs[i].Input));
    });
  }
}
