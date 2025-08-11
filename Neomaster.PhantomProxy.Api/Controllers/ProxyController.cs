using System.Net.Mime;
using System.Security.Cryptography;
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
  private static readonly string _publicPem;
  private static readonly string _privatePem;
  private static readonly string[] _fileMimeTypes;
  private static readonly string[] _textMimeTypes;

  static ProxyController()
  {
    var rsa = RSA.Create(4096);
    _publicPem = rsa.ExportSubjectPublicKeyInfoPem();
    _privatePem = rsa.ExportPkcs8PrivateKeyPem();

    _textMimeTypes = GetConstValues(typeof(MediaTypeNames.Text));
    _fileMimeTypes = GetConstValues(typeof(MediaTypeNames.Font), typeof(MediaTypeNames.Image));
  }

  /// <summary>
  /// Returns RSA SPKI PEM.
  /// </summary>
  /// <returns>RSA SPKI PEM.</returns>
  [HttpGet("/rsa-spki-pem")]
  public string GetRsaSpkiPem()
  {
    return _publicPem;
  }

  /// <summary>
  /// Returns content of given url base64.
  /// </summary>
  /// <param name="url">Base64-encoded encrypted target URL.</param>
  /// <param name="key">Base64-encoded encrypted AES key.</param>
  /// <param name="iv">Base64-encoded IV.</param>
  /// <returns>Content.</returns>
  [HttpGet("/browse")]
  public async Task<IActionResult> BrowseAsync(string url, string key, string iv)
  {
    var encryptedUrlBytes = Convert.FromBase64String(url);
    var encryptedAesKeyBytes = Convert.FromBase64String(key);
    var ivBytes = Convert.FromBase64String(iv);

    using var rsa = RSA.Create();
    rsa.ImportFromPem(_privatePem);
    var aesKeyBytes = rsa.Decrypt(encryptedAesKeyBytes, RSAEncryptionPadding.OaepSHA256);

    var urlDecryptedBytes = AesGcmEncryptor.Decrypt(encryptedUrlBytes, aesKeyBytes, ivBytes);
    url = Encoding.UTF8.GetString(urlDecryptedBytes);

    var request = new ProxyRequest { Url = url };
    var response = await proxyService.ProxyRequestHtmlContentAsync(request);
    var proxyBaseUrl = $"{Request.Scheme}://{Request.Host}/browse?url=";

    var contentBytes = response.ContentBytes;
    var contentText = Encoding.UTF8.GetString(contentBytes);

    if (response.ContentType == MediaTypeNames.Text.Html)
    {
      contentText = proxyService.ProxyHtmlUrls(contentText, new Uri(url), proxyBaseUrl);
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
