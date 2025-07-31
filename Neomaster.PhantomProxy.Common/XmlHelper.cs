using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace Neomaster.PhantomProxy.Common;

/// <summary>
/// Helper methods for working with XML type documents.
/// </summary>
public static class XmlHelper
{
  /// <summary>
  /// Finds HTML attributes by names.
  /// </summary>
  /// <param name="htmlDocument">HTML document to search.</param>
  /// <param name="names">Attribute names to match.</param>
  /// <returns>Matching HTML attributes.</returns>
  public static IEnumerable<HtmlAttribute> GetHtmlAttributes(
    this HtmlDocument htmlDocument,
    IEnumerable<string> names)
  {
    var attrs = htmlDocument.DocumentNode
      .DescendantsAndSelf()
      .SelectMany(n => n.Attributes)
      .Where(a =>
      {
        var name = a.Name.Split(':')[0];
        return names.Contains(name, StringComparer.OrdinalIgnoreCase);
      });

    return attrs;
  }

  /// <summary>
  /// Parses all <c>srcset</c> attribute values.
  /// </summary>
  /// <param name="htmlDocument">HTML document to search.</param>
  /// <returns>Parsed all <c>srcset</c> value items.</returns>
  public static IEnumerable<SrcsetValueItem> GetAllSrcsetValueItems(
    this HtmlDocument htmlDocument)
  {
    var attrs = htmlDocument.GetHtmlAttributes([CommonConsts.XmlAttributeNames.SrcSet]);

    foreach (var attr in attrs)
    {
      var valueItems = attr.DeEntitizeValue
        .Split(',', StringSplitOptions.RemoveEmptyEntries)
        .Select(v => v.Trim())
        .Where(v => !string.IsNullOrEmpty(v));

      foreach (var vi in valueItems)
      {
        var parts = Regex.Split(vi, @"\s+")
          .Where(p => !string.IsNullOrWhiteSpace(p))
          .ToArray();

        var item = new SrcsetValueItem
        {
          Url = parts[0],
          Descriptor = parts.Length > 1 ? parts[1] : string.Empty,
        };

        yield return item;
      }
    }
  }
}
