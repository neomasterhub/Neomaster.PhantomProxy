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
  [InlineData("https://a", "https://b", "https://a")]
  [InlineData("a", "https://b", "https://b/a")]
  public void ToAbsolute_ShouldReturnAbsoluteUri(string url, string baseUrl, string expected)
  {
    var uri = UrlHelper.TryCreateUri(url);
    var baseUri = UrlHelper.TryCreateUri(baseUrl);

    var absoluteUri = uri!.ToAbsolute(baseUri!);

    Assert.True(absoluteUri.IsAbsoluteUri);
    Assert.Equal(expected, absoluteUri.OriginalString);
  }

  [Fact]
  public void ToAbsolute_ShouldThrow_BaseUriIsNotAbsolute()
  {
    var uri = UrlHelper.TryCreateUri("a");
    var baseUri = UrlHelper.TryCreateUri("b");

    Assert.Throws<ArgumentException>(() => uri!.ToAbsolute(baseUri!));
  }
}
