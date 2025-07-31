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
  public string ProxyHtmlUrls(string htmlDoc, Uri baseUrl, string proxyUrlPrefix)
  {
    ArgumentNullException.ThrowIfNull(baseUrl);
    ArgumentException.ThrowIfNullOrWhiteSpace(htmlDoc);
    ArgumentException.ThrowIfNullOrWhiteSpace(proxyUrlPrefix);

    var doc = new HtmlDocument();
    doc.LoadHtml(htmlDoc);

    var attrs = doc.GetHtmlAttributes(settings.UrlAttributeNames);
    var ignoredUrlPrefixes = InfraConsts.IgnoredUrlPrefixes
      .Append(proxyUrlPrefix)
      .ToArray();

    foreach (var attr in attrs)
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

      if (!Uri.TryCreate(attrValue, UriKind.RelativeOrAbsolute, out var uri))
      {
        continue;
      }

      if (!uri.IsAbsoluteUri)
      {
        uri = new Uri(baseUrl, uri);
      }

      var targetUrlBytes = AesGcmEncryptor.Encrypt(Encoding.UTF8.GetBytes(uri.AbsoluteUri), settings.EncryptionPassword);
      var targetUrl = Uri.EscapeDataString(Convert.ToBase64String(targetUrlBytes));
      var proxiedUrl = $"{proxyUrlPrefix}{targetUrl}";
      attr.Value = proxiedUrl;
    }

    var rewrittenHtmlDoc = doc.DocumentNode.OuterHtml;

    return rewrittenHtmlDoc;
  }
}
