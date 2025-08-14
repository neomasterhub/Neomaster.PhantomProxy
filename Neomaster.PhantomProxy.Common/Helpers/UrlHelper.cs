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
    if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri))
    {
      return uri;
    }

    return null;
  }
}
