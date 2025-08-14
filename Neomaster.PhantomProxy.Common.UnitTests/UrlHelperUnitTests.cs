namespace Neomaster.PhantomProxy.Common.UnitTests;

public class UrlHelperUnitTests
{
  [Theory]
  [InlineData("https://example.com")]
  [InlineData("https://example.com/")]
  [InlineData(" https://example.com ")]
  [InlineData("rel/path")]
  [InlineData("rel/path/")]
  [InlineData("/rel/path")]
  [InlineData("/rel/path/")]
  [InlineData(" rel/path ")]
  public void TryCreateUri_ShouldReturnUri_ValidUrl(string url)
  {
    var uri = UrlHelper.TryCreateUri(url);

    Assert.NotNull(uri);
    Assert.Equal(url.Trim(), uri.OriginalString);
  }

  [Theory]
  [InlineData("")]
  [InlineData(" ")]
  public void TryCreateUri_ShouldReturnNull_InvalidUrl(string url)
  {
    var uri = UrlHelper.TryCreateUri(url);

    Assert.Null(uri);
  }

  [Theory]
  [InlineData("https://example.com")]
  [InlineData("https://example.com/")]
  [InlineData(" https://example.com ")]
  [InlineData("rel/path")]
  [InlineData("rel/path/")]
  [InlineData("/rel/path")]
  [InlineData("/rel/path/")]
  [InlineData(" rel/path ")]
  public void CreateUri_ShouldReturnUri_ValidUrl(string url)
  {
    var expectedUri = UrlHelper.TryCreateUri(url);

    var uri = UrlHelper.CreateUri(url);

    Assert.NotNull(expectedUri);
    Assert.Equal(expectedUri, uri);
  }

  [Theory]
  [InlineData("")]
  [InlineData(" ")]
  public void CreateUri_ShouldThrow_InvalidUrl(string url)
  {
    Assert.Throws<ArgumentException>(() => UrlHelper.CreateUri(url));
  }

  [Theory]
  [InlineData("https://a", "https://b", "https://a")]
  [InlineData("a", "https://b", "https://b/a")]
  public void ToAbsolute_ShouldReturnAbsoluteUri(string url, string baseUrl, string expectedUrl)
  {
    var uri = UrlHelper.CreateUri(url);
    var baseUri = UrlHelper.CreateUri(baseUrl);

    var absoluteUri = uri.ToAbsolute(baseUri);

    Assert.True(absoluteUri.IsAbsoluteUri);
    Assert.Equal(expectedUrl, absoluteUri.OriginalString);
  }

  [Fact]
  public void ToAbsolute_ShouldThrow_BaseUriIsNotAbsolute()
  {
    var uri = UrlHelper.CreateUri("a");
    var baseUri = UrlHelper.CreateUri("b");

    Assert.Throws<ArgumentException>(() => uri.ToAbsolute(baseUri));
  }
}
