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
  /// Returns index page.
  /// </summary>
  /// <returns>Index page.</returns>
  [HttpGet("/")]
  public IActionResult Index()
  {
    var path = Path.Combine(Directory.GetCurrentDirectory(), "index.html");
    var html = System.IO.File.ReadAllText(path);

    return Content(html, MediaTypeNames.Text.Html);
  }

  /// <summary>
  /// Returns raw html content of given url.
  /// </summary>
  /// <param name="url">Target url.</param>
  /// <returns>Raw html content.</returns>
  [HttpGet("/browse")]
  public async Task<IActionResult> BrowseAsync([FromQuery] string url)
  {
    var request = new HtmlContentProxyRequest { Url = url };
    var response = await proxyService.ProxyRequestHtmlContentAsync(request);
    var proxyBaseUrl = $"{Request.Scheme}://{Request.Host}/browse?url=";

    if (_fileMimeTypes.Contains(response.ContentType))
    {
      var contentBytes = response.ContentBytes;
      if (response.ContentType == MediaTypeNames.Image.Svg)
      {
        var contentText = Encoding.UTF8.GetString(response.ContentBytes);
        contentText = proxyService.RewriteLinksWithProxyUrls(contentText, new Uri(url), proxyBaseUrl);
        contentBytes = Encoding.UTF8.GetBytes(contentText);
      }

      return File(contentBytes, response.ContentType);
    }

    if (_textMimeTypes.Contains(response.ContentType))
    {
      var contentText = Encoding.UTF8.GetString(response.ContentBytes);

      if (response.ContentType == MediaTypeNames.Text.Html)
      {
        contentText = proxyService.RewriteLinksWithProxyUrls(contentText, new Uri(url), proxyBaseUrl);
      }

      return Content(contentText, response.ContentType);
    }

    return StatusCode(StatusCodes.Status415UnsupportedMediaType, $"Unsupported content type: {response.ContentType}.");
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
