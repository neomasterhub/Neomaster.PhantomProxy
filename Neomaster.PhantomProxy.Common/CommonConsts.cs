namespace Neomaster.PhantomProxy.Common;

/// <summary>
/// Common-level constants.
/// </summary>
public class CommonConsts
{
  /// <summary>
  /// Regular expression patterns.
  /// </summary>
  public class RegexPatterns
  {
    /// <summary>
    /// Regular expression pattern for matching <c>url()</c> function.
    /// </summary>
    public const string UrlFunction = @"url\(\s*(?:(['""])(?<url>.*?)\1|(?<url>[^)]+))\s*\)";

    /// <summary>
    /// Regular expression pattern for matching CSS <c>@import</c> statement.
    /// </summary>
    public const string CssImport = @"@import\s+(?:url\(\s*(?<quote>['""]?)(?<url>[^'"")]+)\k<quote>\s*\)|(?<quote2>['""]?)(?<url2>[^'"")\s]+)\k<quote2>)";
  }

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
