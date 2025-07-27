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
}
