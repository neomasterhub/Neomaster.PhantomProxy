using System.Text;

namespace Neomaster.PhantomProxy.Common.UnitTests;

public class ContentHelperUnitTests
{
  [Fact]
  public void TryGetBomEncoding_ShouldReturnBomEncoding_BomIsPresent()
  {
    var aByte = (byte)'a';
    var testCases = new (
      IEnumerable<byte> ContentBom,
      IEnumerable<byte> ContentPayload,
      Encoding ExpectedEncoding)[]
    {
      (ContentHelper.Utf8Bom, new[] { aByte }, Encoding.UTF8),
      (ContentHelper.Utf16LeBom, new byte[] { aByte, 0x00 }, Encoding.Unicode),
      (ContentHelper.Utf16BeBom, new byte[] { 0x00, aByte }, Encoding.BigEndianUnicode),
      (ContentHelper.Utf32LeBom, new byte[] { aByte, 0x00, 0x00, 0x00 }, new UTF32Encoding(false, true)),
      (ContentHelper.Utf32BeBom, new byte[] { 0x00, 0x00, 0x00, aByte }, new UTF32Encoding(true, true)),
    };

    foreach (var (contentBom, contentPayload, expectedEncoding) in testCases)
    {
      var content = contentBom.Concat(contentPayload).ToArray();

      var actualEncoding = ContentHelper.TryGetBomEncoding(content);

      Assert.NotNull(actualEncoding);
      Assert.Equal(expectedEncoding.EncodingName, actualEncoding.EncodingName);
    }
  }

  [Fact]
  public void TryGetBomEncoding_ShouldReturnNull_BomIsNotPresent()
  {
    var actualEncoding = ContentHelper.TryGetBomEncoding("a"u8);

    Assert.Null(actualEncoding);
  }
}
