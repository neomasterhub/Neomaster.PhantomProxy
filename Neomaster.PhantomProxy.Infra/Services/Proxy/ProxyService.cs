using System.Net;
using HtmlAgilityPack;
using Neomaster.PhantomProxy.App;

namespace Neomaster.PhantomProxy.Infra;

/// <inheritdoc/>
public class ProxyService(
  IHttpClientFactory httpClientFactory)
  : IProxyService
{
  private readonly HttpClient _httpClient = httpClientFactory.CreateClient(nameof(PhantomProxy));

  /// <inheritdoc/>
  public async Task<HtmlContentProxyResponse> ProxyRequestHtmlContentAsync(HtmlContentProxyRequest request)
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

    var result = new HtmlContentProxyResponse
    {
      ContentBytes = await responseMessage.Content.ReadAsByteArrayAsync(),
      ContentType = responseMessage.GetContentType(url).ToLower(),
    };

    return result;
  }

  /// <inheritdoc/>
  public string RewriteLinksWithProxyUrls(string htmlDoc, Uri baseUrl, string proxyUrlPrefix)
  {
    ArgumentNullException.ThrowIfNull(baseUrl);
    ArgumentException.ThrowIfNullOrWhiteSpace(htmlDoc);
    ArgumentException.ThrowIfNullOrWhiteSpace(proxyUrlPrefix);

    var doc = new HtmlDocument();
    doc.LoadHtml(htmlDoc);

    RewriteAttributeValuesWithProxyUrls(doc, "href", baseUrl, proxyUrlPrefix);
    RewriteAttributeValuesWithProxyUrls(doc, "src", baseUrl, proxyUrlPrefix);

    var rewrittenHtmlDoc = doc.DocumentNode.OuterHtml;

    return rewrittenHtmlDoc;
  }

  /// <inheritdoc/>
  public void RewriteAttributeValuesWithProxyUrls(HtmlDocument doc, string attrName, Uri baseUri, string proxyUrlPrefix)
  {
    ArgumentNullException.ThrowIfNull(doc);
    ArgumentException.ThrowIfNullOrWhiteSpace(attrName);
    ArgumentNullException.ThrowIfNull(baseUri);
    ArgumentException.ThrowIfNullOrWhiteSpace(proxyUrlPrefix);

    var nodes = doc.DocumentNode.SelectNodes($"//*[@{attrName}]");
    if (nodes == null)
    {
      return;
    }

    foreach (var node in nodes)
    {
      var attrValue = node.GetAttributeValue(attrName, null!);
      if (string.IsNullOrWhiteSpace(attrValue)
        || attrValue.StartsWith('#'))
      {
        continue;
      }

      attrValue = WebUtility.HtmlDecode(attrValue);

      if (!Uri.TryCreate(attrValue, UriKind.RelativeOrAbsolute, out var uri))
      {
        continue;
      }

      if (!uri.IsAbsoluteUri)
      {
        uri = new Uri(baseUri, uri);
      }

      var proxiedUrl = $"{proxyUrlPrefix}{Uri.EscapeDataString(uri.AbsoluteUri)}";
      node.SetAttributeValue(attrName, proxiedUrl);
    }
  }
}
