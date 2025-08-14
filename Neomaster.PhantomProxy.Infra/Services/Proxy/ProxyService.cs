using System.Text;
using HtmlAgilityPack;
using Neomaster.PhantomProxy.App;
using Neomaster.PhantomProxy.Common;

namespace Neomaster.PhantomProxy.Infra;

/// <inheritdoc/>
public class ProxyService(
  PhantomProxySettings settings,
  IHttpClientFactory httpClientFactory)
  : IProxyService
{
  private readonly HttpClient _httpClient = httpClientFactory.CreateClient(nameof(PhantomProxy));

  /// <inheritdoc/>
  public async Task<ProxyResponse> ProxyRequestHtmlContentAsync(ProxyRequest request)
  {
    if (string.IsNullOrWhiteSpace(request.Url))
    {
      throw new ArgumentException(InfraConsts.ErrorMessages.UrlEmpty, nameof(request.Url));
    }

    var url = request.Url.Trim();

    if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
    {
      throw new ArgumentException(InfraConsts.ErrorMessages.UrlInvalidFormat, nameof(request.Url));
    }

    var responseMessage = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
    responseMessage.EnsureSuccessStatusCode();

    var result = new ProxyResponse
    {
      ContentBytes = await responseMessage.Content.ReadAsByteArrayAsync(),
      ContentType = responseMessage.GetContentType(url).ToLower(),
    };

    return result;
  }

  /// <inheritdoc/>
  public string ProxyHtmlUrls(string htmlDoc, Uri baseUri, string proxyUrlFormat, byte[] aesKey, byte[] aesIV)
  {
    ArgumentNullException.ThrowIfNull(baseUri);
    ArgumentException.ThrowIfNullOrWhiteSpace(htmlDoc);
    ArgumentException.ThrowIfNullOrWhiteSpace(proxyUrlFormat);

    var doc = new HtmlDocument();
    doc.LoadHtml(htmlDoc);

    var proxyUrlPrefix = proxyUrlFormat.Split('?', 2)[0];
    var ignoredUrlPrefixes = InfraConsts.IgnoredUrlPrefixes
      .Append(proxyUrlPrefix)
      .ToArray();

    var singleValueAttrs = doc.GetHtmlAttributes(settings.UrlAttributeNames);
    foreach (var attr in singleValueAttrs)
    {
      if (string.IsNullOrWhiteSpace(attr.Value))
      {
        continue;
      }

      var attrValue = attr.DeEntitizeValue.Trim();

      if (ignoredUrlPrefixes.Any(
        prefix => attrValue.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)))
      {
        continue;
      }

      attr.Value = ProxyUrl(attrValue, baseUri, proxyUrlFormat, aesKey, aesIV);
    }

    var allSrcsetValueItems = doc.GetAllSrcsetValueItems();
    foreach (var vi in allSrcsetValueItems)
    {
      vi.HtmlAttribute.Value = vi.HtmlAttribute.DeEntitizeValue.Replace(
        vi.Url,
        ProxyUrl(vi.Url, baseUri, proxyUrlFormat, aesKey, aesIV));
    }

    var rewrittenHtmlDoc = doc.DocumentNode.OuterHtml;

    return rewrittenHtmlDoc;
  }

  /// <inheritdoc/>
  public string ProxyUrl(string url, Uri baseUri, string proxyUrlFormat, byte[] aesKey, byte[] aesIV)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(url);

    var uri = UrlHelper.TryCreateUri(url)?.ToAbsolute(baseUri);
    if (uri == null)
    {
      return url;
    }

    var urlBytes = Encoding.UTF8.GetBytes(uri.AbsoluteUri);
    var urlBytesEncrypted = AesGcmEncryptor.Encrypt(urlBytes, aesKey, aesIV);
    var urlBytesEncryptedBase64Escaped = Uri.EscapeDataString(Convert.ToBase64String(urlBytesEncrypted));

    var proxiedUrl = string.Format(proxyUrlFormat, urlBytesEncryptedBase64Escaped);

    return proxiedUrl;
  }
}
