using System.Net.Mime;
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
  public async Task<IActionResult> BrowseAsync(string url)
  {
    Request.Cookies.TryGetValue("key", out var key);
    Request.Cookies.TryGetValue("iv", out var iv);
    Request.Cookies.TryGetValue("session-id", out var sessionId);

    ArgumentException.ThrowIfNullOrWhiteSpace(key);
    ArgumentException.ThrowIfNullOrWhiteSpace(iv);
    ArgumentException.ThrowIfNullOrWhiteSpace(sessionId);

    var encryptedAesKeyBytes = Convert.FromBase64String(key);
    var ivBytes = Convert.FromBase64String(iv);

    // Decrypt AES key.
    var rsaPems = cacheService.RestoreRsaPems(sessionId);
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

    // Create proxy URL format string.
    var proxyUrlFormat = $"{Request.Scheme}://{Request.Host}/browse?url={{0}}";
    var baseUri = new Uri(url);

    // Prepare content for handling.
    var content = ContentHelper.PrepareContent(response.ContentBytes, response.ContentType);

    if (response.ContentType.StartsWith("text/", StringComparison.OrdinalIgnoreCase))
    {
      var proxiedContentText = proxyService.ProxyUrlFunctionUrls(content.Text, baseUri, proxyUrlFormat, urlEncryptionOptions);
      proxiedContentText = proxyService.ProxyCssImportUrls(content.Text, baseUri, proxyUrlFormat, urlEncryptionOptions);

      if (response.ContentType == MediaTypeNames.Text.Html)
      {
        proxiedContentText = proxyService.ProxyHtmlUrls(proxiedContentText, baseUri, proxyUrlFormat, urlEncryptionOptions);
      }

      return Content(proxiedContentText, content.ContentTypeHeader);
    }

    return File(content.RawBytes, content.Info.MediaType);
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
