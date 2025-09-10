using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Neomaster.PhantomProxy.App;
using Neomaster.PhantomProxy.Common;

namespace Neomaster.PhantomProxy.Infra;

/// <inheritdoc/>
public class ProxyService(
  PhantomProxySettings settings,
  IHttpClientFactory httpClientFactory,
  IUrlEncryptService urlEncryptService)
  : IProxyService
{
  private static readonly Regex _urlFunctionRegex = new(
    CommonConsts.RegexPatterns.UrlFunction,
    RegexOptions.IgnoreCase | RegexOptions.Compiled);

  private static readonly Regex _cssImportRegex = new(
    CommonConsts.RegexPatterns.CssImport,
    RegexOptions.IgnoreCase | RegexOptions.Compiled);

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

    // Handle 429 error.
    // TODO: Change for unit tests.
    // TODO: Add retry number in config file.
    if (responseMessage.StatusCode == HttpStatusCode.TooManyRequests)
    {
      var delta = responseMessage.Headers.RetryAfter?.Delta ?? TimeSpan.FromSeconds(1); // TODO: Move to config file.
      await Task.Delay(delta);

      responseMessage = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
    }

    var result = new ProxyResponse
    {
      ContentBytes = await responseMessage.Content.ReadAsByteArrayAsync(),
      ContentType = responseMessage.GetContentType(url).ToLower(),
    };

    return result;
  }

  /// <inheritdoc/>
  public string ProxyCssImportUrls(string cssText, Uri baseUri, string proxyUrlFormat, EncryptionOptions? encryptionOptions = null)
  {
    ArgumentNullException.ThrowIfNull(baseUri);
    ArgumentException.ThrowIfNullOrWhiteSpace(cssText);
    ArgumentException.ThrowIfNullOrWhiteSpace(proxyUrlFormat);

    var proxiedCssText = _cssImportRegex.Replace(cssText, match =>
    {
      var url = match.Groups["url"].Value.Trim();

      if (string.IsNullOrWhiteSpace(url))
      {
        return match.Value;
      }

      var withUrlFunction = match.Groups["withUrlFunction"].Success;
      var quote = withUrlFunction
        ? match.Groups["quote"].Value
        : match.Groups["quote2"].Value;

      if (string.IsNullOrEmpty(quote) && !withUrlFunction)
      {
        quote = "'";
      }

      var proxiedUrl = ProxyUrl(url, baseUri, proxyUrlFormat, encryptionOptions);

      var cssImportStatement = withUrlFunction
        ? $"@import url({quote}{proxiedUrl}{quote})"
        : $"@import {quote}{proxiedUrl}{quote};";

      return cssImportStatement;
    });

    return proxiedCssText;
  }

  /// <inheritdoc/>
  public string ProxyUrlFunctionUrls(string text, Uri baseUri, string proxyUrlFormat, EncryptionOptions? encryptionOptions = null)
  {
    ArgumentNullException.ThrowIfNull(baseUri);
    ArgumentException.ThrowIfNullOrWhiteSpace(text);
    ArgumentException.ThrowIfNullOrWhiteSpace(proxyUrlFormat);

    var proxiedText = _urlFunctionRegex.Replace(text, match =>
    {
      var url = match.Groups["url"].Value.Trim();

      if (string.IsNullOrWhiteSpace(url))
      {
        return match.Value;
      }

      var quote = match.Groups[1].Success ? match.Groups[1].Value : string.Empty;
      var proxiedUrl = ProxyUrl(url, baseUri, proxyUrlFormat, encryptionOptions);
      var proxiedUrlFunction = $"url({quote}{proxiedUrl}{quote})";

      return proxiedUrlFunction;
    });

    return proxiedText;
  }

  /// <inheritdoc/>
  public string ProxyHtmlUrls(string htmlDoc, Uri baseUri, string proxyUrlFormat, EncryptionOptions? encryptionOptions = null)
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

      attr.Value = ProxyUrl(attrValue, baseUri, proxyUrlFormat, encryptionOptions);
    }

    var allSrcsetValueItems = doc.GetAllSrcsetValueItems();
    foreach (var vi in allSrcsetValueItems)
    {
      vi.HtmlAttribute.Value = vi.HtmlAttribute.DeEntitizeValue.Replace(
        vi.Url,
        ProxyUrl(vi.Url, baseUri, proxyUrlFormat, encryptionOptions));
    }

    var rewrittenHtmlDoc = doc.DocumentNode.OuterHtml;

    return rewrittenHtmlDoc;
  }

  /// <inheritdoc/>
  public string ProxyUrl(string url, Uri baseUri, string proxyUrlFormat, EncryptionOptions? encryptionOptions = null)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(url);

    var uri = UrlHelper.TryCreateUri(url)?.ToAbsolute(baseUri);
    if (uri == null)
    {
      return url;
    }

    var urlEncrypted = urlEncryptService.Encrypt(uri.AbsoluteUri, encryptionOptions);
    var urlProxied = string.Format(proxyUrlFormat, urlEncrypted);

    return urlProxied;
  }
}
