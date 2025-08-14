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
  public string ProxyHtmlUrls(string htmlDoc, Uri baseUrl, string proxyUrlFormat, byte[] aesKey, byte[] aesIV)
  {
    ArgumentNullException.ThrowIfNull(baseUrl);
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

      attr.Value = ProxyUrl(attrValue, baseUrl, proxyUrlFormat, aesKey, aesIV);
    }

    var allSrcSetValueItems = doc.GetAllSrcsetValueItems();
    foreach (var vi in allSrcSetValueItems)
    {
      vi.HtmlAttribute.Value = vi.HtmlAttribute.DeEntitizeValue.Replace(
        vi.Url,
        ProxyUrl(vi.Url, baseUrl, proxyUrlFormat, aesKey, aesIV));
    }

    var rewrittenHtmlDoc = doc.DocumentNode.OuterHtml;

    return rewrittenHtmlDoc;
  }

  /// <inheritdoc/>
  public string ProxyUrl(string url, Uri baseUrl, string proxyUrlFormat, byte[] aesKey, byte[] aesIV)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(url);

    var uri = UrlHelper.TryCreateUri(url)?.ToAbsolute(baseUrl);
    if (uri == null)
    {
      return url;
    }

    var targetUrlBytes = AesGcmEncryptor.Encrypt(Encoding.UTF8.GetBytes(uri.AbsoluteUri), aesKey, aesIV);
    var targetUrl = Uri.EscapeDataString(Convert.ToBase64String(targetUrlBytes));
    var proxiedUrl = string.Format(proxyUrlFormat, targetUrl);

    return proxiedUrl;
  }
}
