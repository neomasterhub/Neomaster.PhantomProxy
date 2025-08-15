using System.Net.Mime;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Neomaster.PhantomProxy.App;
using Neomaster.PhantomProxy.Common;

namespace Neomaster.PhantomProxy.Api;

/// <summary>
/// Controller for anonymously proxying requests.
/// </summary>
public class ProxyController(
  ICacheService cacheService,
  IProxyService proxyService,
  ISessionService sessionService,
  IUrlEncryptService urlEncryptService)
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
  /// Starts new session and returns session information.
  /// </summary>
  /// <returns>Session information.</returns>
  [HttpGet("/start-session")]
  public SessionInfo StartSession()
  {
    var sessionInfo = sessionService.Start();

    return sessionInfo;
  }

  /// <summary>
  /// Returns content of given url base64.
  /// </summary>
  /// <param name="url">Base64-encoded encrypted target URL.</param>
  /// <param name="key">Base64-encoded encrypted AES key.</param>
  /// <param name="iv">Base64-encoded IV.</param>
  /// <param name="pem">RSA PEM-encoded key.</param>
  /// <returns>Content.</returns>
  [HttpGet("/browse")]
  public async Task<IActionResult> BrowseAsync(string url, string key, string iv, string pem)
  {
    var encryptedAesKeyBytes = Convert.FromBase64String(key);
    var ivBytes = Convert.FromBase64String(iv);

    // Decrypt AES key.
    var rsaPems = cacheService.RestoreRsaPems(pem);
    var aesKeyBytes = RsaEncryptor.Decrypt(encryptedAesKeyBytes, rsaPems.PrivatePem);

    // Decrypt URL.
    var urlEncryptionOptions = new EncryptionOptions
    {
      AesKey = aesKeyBytes,
      AesIV = ivBytes,
    };
    url = urlEncryptService.Decrypt(url, urlEncryptionOptions);

    // Request content.
    var request = new ProxyRequest { Url = url };
    var response = await proxyService.ProxyRequestHtmlContentAsync(request);

    // Prepare content for handling.
    var contentBytes = response.ContentBytes;
    var contentText = Encoding.UTF8.GetString(contentBytes);

    // Create proxy URL format string.
    iv = Uri.EscapeDataString(iv);
    key = Uri.EscapeDataString(key);
    pem = Uri.EscapeDataString(pem);
    var proxyUrlFormat = $"{Request.Scheme}://{Request.Host}/browse?url={{0}}&key={key}&iv={iv}&pem={pem}";
    var baseUri = new Uri(url);

    contentText = proxyService.ProxyUrlFunctionUrls(contentText, baseUri, proxyUrlFormat, urlEncryptionOptions);

    if (response.ContentType == MediaTypeNames.Text.Html)
    {
      contentText = proxyService.ProxyHtmlUrls(contentText, baseUri, proxyUrlFormat, urlEncryptionOptions);
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
