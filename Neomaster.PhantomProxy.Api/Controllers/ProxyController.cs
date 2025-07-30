using System.Net.Mime;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Neomaster.PhantomProxy.App;
using Neomaster.PhantomProxy.Common;
using Neomaster.PhantomProxy.Infra;

namespace Neomaster.PhantomProxy.Api;

/// <summary>
/// Controller for anonymously proxying requests.
/// </summary>
public class ProxyController(
  IProxyService proxyService,
  PhantomProxySettings settings)
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
    var html = System.IO.File
      .ReadAllText(path)
      .Replace("$password$", settings.EncryptionPassword);

    return Content(html, MediaTypeNames.Text.Html);
  }

  /// <summary>
  /// Returns content of given url base64.
  /// </summary>
  /// <param name="url">Base64-encoded encrypted target URL.</param>
  /// <returns>Content.</returns>
  [HttpGet("/browse")]
  public async Task<IActionResult> BrowseAsync([FromQuery] string url)
  {
    var urlDecryptedBytes = AesGcmEncryptor.Decrypt(Convert.FromBase64String(url), settings.EncryptionPassword);
    url = Encoding.UTF8.GetString(urlDecryptedBytes);

    var request = new HtmlContentProxyRequest { Url = url };
    var response = await proxyService.ProxyRequestHtmlContentAsync(request);
    var proxyBaseUrl = $"{Request.Scheme}://{Request.Host}/browse?url=";

    var contentBytes = response.ContentBytes;
    var contentText = Encoding.UTF8.GetString(contentBytes);

    if (response.ContentType == MediaTypeNames.Text.Html)
    {
      contentText = proxyService.RewriteLinksWithProxyUrls(contentText, new Uri(url), proxyBaseUrl);
    }

    if (_fileMimeTypes.Contains(response.ContentType))
    {
      return File(contentBytes, response.ContentType);
    }

    if (_textMimeTypes.Contains(response.ContentType))
    {
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
