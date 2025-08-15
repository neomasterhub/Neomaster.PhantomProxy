namespace Neomaster.PhantomProxy.Common;

/// <summary>
/// Common-level constants.
/// </summary>
public class CommonConsts
{
  /// <summary>
  /// Regular expression pattern for matching <c>url()</c> function.
  /// </summary>
  public const string UrlFunctionRegexPattern = @"url\(\s*(?:(['""])(?<url>.*?)\1|(?<url>[^)]+))\s*\)";

  /// <summary>
  /// XML structure attribute names.
  /// </summary>
  public class XmlAttributeNames
  {
    /// <summary>
    /// <a href="https://html.spec.whatwg.org/multipage/images.html#srcset-attribute"><c>srcset</c></a> attribute name.
    /// </summary>
    public const string SrcSet = "srcset";
  }
}
