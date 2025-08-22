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
