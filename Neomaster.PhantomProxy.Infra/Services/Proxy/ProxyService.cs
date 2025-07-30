using HtmlAgilityPack;
using Neomaster.PhantomProxy.App;

namespace Neomaster.PhantomProxy.Infra;

/// <inheritdoc/>
public class ProxyService(
  PhantomProxySettings settings,
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

    RewriteAttributeValuesWithProxyUrls(doc, baseUrl, proxyUrlPrefix);

    var rewrittenHtmlDoc = doc.DocumentNode.OuterHtml;

    return rewrittenHtmlDoc;
  }

  /// <inheritdoc/>
  public void RewriteAttributeValuesWithProxyUrls(HtmlDocument doc, Uri baseUri, string proxyUrlPrefix)
  {
    ArgumentNullException.ThrowIfNull(doc);
    ArgumentNullException.ThrowIfNull(baseUri);
    ArgumentException.ThrowIfNullOrWhiteSpace(proxyUrlPrefix);

    var attrs = doc.DocumentNode
      .DescendantsAndSelf()
      .SelectMany(n => n.Attributes)
      .Where(a => settings.UrlAttributeNames.Contains(a.Name));

    foreach (var attr in attrs)
    {
      var attrValue = attr.DeEntitizeValue;

      if (string.IsNullOrWhiteSpace(attrValue)
        || attrValue.StartsWith('#')
        || attrValue.StartsWith(proxyUrlPrefix))
      {
        continue;
      }

      if (!Uri.TryCreate(attrValue, UriKind.RelativeOrAbsolute, out var uri))
      {
        continue;
      }

      if (!uri.IsAbsoluteUri)
      {
        uri = new Uri(baseUri, uri);
      }

      var proxiedUrl = $"{proxyUrlPrefix}{Uri.EscapeDataString(uri.AbsoluteUri)}";
      attr.Value = proxiedUrl;
    }
  }
}
