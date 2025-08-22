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
  public byte[] ContentBytes { get; init; } = [];

  /// <summary>
  /// Content information from HTTP response.
  /// </summary>
  public ContentInfo ContentInfo { get; init; } = new();

  /// <summary>
  /// Detected content encoding.
  /// </summary>
  public Encoding ContentEncoding { get; init; } = Encoding.UTF8;

  /// <summary>
  /// Content text in detected encoding.
  /// </summary>
  public string ContentText => ContentEncoding.GetString(ContentBytes);

  /// <summary>
  /// Content-Type header value with detected media type and charset.
  /// </summary>
  public string ContentType => $"{ContentInfo.MediaType}; {ContentInfo.Charset}";
}
