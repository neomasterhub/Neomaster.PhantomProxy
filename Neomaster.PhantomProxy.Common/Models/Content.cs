using System.Text;

namespace Neomaster.PhantomProxy.Common;

/// <summary>
/// Represents content from HTTP response.
/// </summary>
public record Content
{
  /// <summary>
  /// Raw content bytes.
  /// </summary>
  public byte[] RawBytes { get; init; } = [];

  /// <summary>
  /// Raw content bytes with BOM.
  /// </summary>
  public bool WithBom { get; init; }

  /// <summary>
  /// Content information from HTTP response.
  /// </summary>
  public ContentInfo Info { get; init; } = new();

  /// <summary>
  /// Detected content encoding.
  /// </summary>
  public Encoding Encoding { get; init; } = Encoding.UTF8;

  /// <summary>
  /// Content text in detected encoding.
  /// </summary>
  public string Text => Encoding.GetString(RawBytes);

  /// <summary>
  /// Content-Type header value with detected media type and charset.
  /// </summary>
  public string ContentTypeHeader => $"{Info.MediaType}; {Encoding.WebName}";
}
