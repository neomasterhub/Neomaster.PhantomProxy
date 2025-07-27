using System.Net.Mime;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Neomaster.PhantomProxy.App;

namespace Neomaster.PhantomProxy.Api;

/// <summary>
/// Controller for anonymously proxying requests.
/// </summary>
public class ProxyController(
  IProxyService proxyService)
  : ApiControllerBase
{
  private static readonly string[] _fileMimeTypes;
  private static readonly string[] _textMimeTypes;

  static ProxyController()
  {
    _textMimeTypes = GetConstValues(typeof(MediaTypeNames.Text));
    _fileMimeTypes = GetConstValues(typeof(MediaTypeNames.Font), typeof(MediaTypeNames.Image));
  }

  /// <summary>
  /// Returns raw html content of given url.
  /// </summary>
  /// <param name="url">Target url.</param>
  /// <returns>Raw html content.</returns>
  [HttpGet("browse")]
  public async Task<IActionResult> BrowseAsync([FromQuery] string url)
  {
    var request = new HtmlContentProxyRequest { Url = url };
    var response = await proxyService.ProxyRequestHtmlContentAsync(request);

    if (_fileMimeTypes.Contains(response.ContentType))
    {
      return File(response.ContentBytes, response.ContentType);
    }

    if (_textMimeTypes.Contains(response.ContentType))
    {
      var content = Encoding.UTF8.GetString(response.ContentBytes);

      if (response.ContentType == MediaTypeNames.Text.Html)
      {
        // TODO: Proxy links in HTML content.
        // TODO: Handle <base>?
      }

      return Content(content, response.ContentType);
    }

    throw new NotSupportedException($"Response contains unsupported type content: {response.ContentType}.");
  }

  private static string[] GetConstValues(params Type[] types)
  {
    var values = types
      .SelectMany(t => t.GetFields())
      .Select(f => (f.GetRawConstantValue() as string)!)
      .ToArray();

    return values;
  }
}
