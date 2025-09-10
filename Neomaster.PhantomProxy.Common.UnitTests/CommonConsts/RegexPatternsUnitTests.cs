using System.Text.RegularExpressions;

namespace Neomaster.PhantomProxy.Common.UnitTests;

public class RegexPatternsUnitTests
{
  private static readonly Regex _urlFunctionRegex = new(CommonConsts.RegexPatterns.UrlFunction, RegexOptions.Compiled);

  [Theory]
  [InlineData("url(x)")]
  [InlineData("url('x')")]
  [InlineData("url(\"x\")")]
  [InlineData("url ( x )")]
  public void UrlFunctionRegex_ShouldMatch(string input)
  {
    const string expectedUrl = "x";

    var match = _urlFunctionRegex.Match(input);

    Assert.True(match.Success);
    Assert.Equal(expectedUrl, match.Groups["url"].Value);
  }
}
