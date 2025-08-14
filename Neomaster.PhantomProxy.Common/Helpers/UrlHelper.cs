namespace Neomaster.PhantomProxy.Common;

/// <summary>
/// Helper methods for working with URLs.
/// </summary>
public static class UrlHelper
{
  /// <summary>
  /// Creates <see cref="Uri"/> from string URL.
  /// </summary>
  /// <param name="url">Absolute or relative string URL.</param>
  /// <returns><see cref="Uri"/> if given string URL is valid, otherwise <c>null</c>.</returns>
  public static Uri? TryCreateUri(string url)
  {
    if (string.IsNullOrWhiteSpace(url))
    {
      return null;
    }

    url = url.Trim();

    if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri))
    {
      return uri;
    }

    return null;
  }

  /// <summary>
  /// Converts given <see cref="Uri"/> to absolute.
  /// </summary>
  /// <param name="uri">URI to convert to absolute.</param>
  /// <param name="baseUri">Absolute URI used to resolve relative URI.</param>
  /// <returns>Absolute <see cref="Uri"/>.</returns>
  public static Uri ToAbsolute(this Uri uri, Uri baseUri)
  {
    ArgumentNullException.ThrowIfNull(uri);
    ArgumentNullException.ThrowIfNull(baseUri);

    if (!baseUri.IsAbsoluteUri)
    {
      throw new ArgumentException("Base URI must be absolute.", nameof(baseUri));
    }

    if (uri.IsAbsoluteUri)
    {
      return uri;
    }

    return new Uri(baseUri, uri);
  }
}
