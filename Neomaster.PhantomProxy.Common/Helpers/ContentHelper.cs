using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;

namespace Neomaster.PhantomProxy.Common;

/// <summary>
/// Helper methods for handling content.
/// </summary>
public static class ContentHelper
{
  public static readonly byte[] Utf8Bom = [0xEF, 0xBB, 0xBF];
  public static readonly byte[] Utf16LeBom = [0xFF, 0xFE];
  public static readonly byte[] Utf16BeBom = [0xFE, 0xFF];
  public static readonly byte[] Utf32LeBom = [0xFF, 0xFE, 0x00, 0x00];
  public static readonly byte[] Utf32BeBom = [0x00, 0x00, 0xFE, 0xFF];

  /// <summary>
  /// Returns content data from HTTP response, prepared for business logic.
  /// </summary>
  /// <param name="rawBytes">HTTP response content bytes.</param>
  /// <param name="contentType">Content-Type header value.</param>
  /// <returns>Content data ready for business logic handling.</returns>
  public static Content PrepareContent(byte[] rawBytes, string contentType)
  {
    var contentInfo = GetContentInfo(contentType);
    var encoding = TryGetBomEncoding(rawBytes) ?? Encoding.GetEncoding(contentInfo.Charset);
    var result = new Content
    {
      ContentBytes = rawBytes,
      ContentEncoding = encoding,
      ContentInfo = contentInfo,
    };

    return result;
  }

  /// <summary>
  /// Returns content information from Content-Type header value.
  /// </summary>
  /// <param name="contentType">Content-Type header value.</param>
  /// <returns>Content information.</returns>
  public static ContentInfo GetContentInfo(string contentType)
  {
    if (MediaTypeHeaderValue.TryParse(contentType, out var mt))
    {
      return new ContentInfo
      {
        Charset = mt.CharSet ?? Encoding.UTF8.WebName,
        MediaType = mt.MediaType ?? MediaTypeNames.Application.Octet,
      };
    }

    return new ContentInfo
    {
      Charset = Encoding.UTF8.WebName,
      MediaType = MediaTypeNames.Application.Octet,
    };
  }

  /// <summary>
  /// Returns encoding from BOM or null.
  /// </summary>
  /// <param name="bytes">Content bytes.</param>
  /// <returns>Encoding or null.</returns>
  public static Encoding? TryGetBomEncoding(ReadOnlySpan<byte> bytes)
  {
    if (bytes.StartsWith(Utf8Bom))
    {
      return Encoding.UTF8;
    }

    if (bytes.StartsWith(Utf32LeBom))
    {
      return new UTF32Encoding(false, true);
    }

    if (bytes.StartsWith(Utf32BeBom))
    {
      return new UTF32Encoding(true, true);
    }

    if (bytes.StartsWith(Utf16LeBom))
    {
      return Encoding.Unicode;
    }

    if (bytes.StartsWith(Utf16BeBom))
    {
      return Encoding.BigEndianUnicode;
    }

    return null;
  }
}
