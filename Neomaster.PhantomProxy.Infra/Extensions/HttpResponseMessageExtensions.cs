using HeyRed.Mime;

namespace Neomaster.PhantomProxy.Infra;

/// <summary>
/// Extensions for <see cref="HttpResponseMessage"/>.
/// </summary>
public static class HttpResponseMessageExtensions
{
  /// <summary>
  /// Gets Content-Type from response or URL extension.
  /// </summary>
  /// <param name="responseMessage">HTTP response message.</param>
  /// <param name="requestUrl">Request URL.</param>
  /// <returns>Content-Type from response or URL extension.</returns>
  public static string GetContentType(this HttpResponseMessage responseMessage, string requestUrl)
  {
    var contentType = responseMessage.Content.Headers.ContentType?.MediaType
      ?? MimeTypesMap.GetMimeType(requestUrl);

    return contentType;
  }
}
