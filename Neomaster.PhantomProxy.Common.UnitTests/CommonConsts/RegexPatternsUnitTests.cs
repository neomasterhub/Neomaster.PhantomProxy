using System.Text.RegularExpressions;

namespace Neomaster.PhantomProxy.Common.UnitTests;

public class RegexPatternsUnitTests
{
  private static readonly Regex _cssImport = new(CommonConsts.RegexPatterns.CssImport, RegexOptions.Compiled);
  private static readonly Regex _urlFunctionRegex = new(CommonConsts.RegexPatterns.UrlFunction, RegexOptions.Compiled);

  [Theory]
  [InlineData("url(x)")]
  [InlineData("url('x')")]
  [InlineData("url(\"x\")")]
  public void UrlFunctionRegex_ShouldMatch(string input)
  {
    const string expectedUrl = "x";

    var match = _urlFunctionRegex.Match(input);

    Assert.True(match.Success);
    Assert.Equal(expectedUrl, match.Groups["url"].Value);
  }

  [Theory]
  [InlineData("@import x")]
  [InlineData("@import 'x'")]
  [InlineData("@import \"x\"")]
  [InlineData("@import x;")]
  [InlineData("@import 'x';")]
  [InlineData("@import \"x\";")]
  [InlineData("@import x ;")]
  [InlineData("@import 'x' ;")]
  [InlineData("@import \"x\" ;")]
  [InlineData("@import x y")]
  [InlineData("@import 'x' y")]
  [InlineData("@import \"x\" y")]
  [InlineData("@import url(x)")]
  [InlineData("@import url('x')")]
  [InlineData("@import url(\"x\")")]
  [InlineData("@import url( x )")]
  [InlineData("@import url( 'x' )")]
  [InlineData("@import url( \"x\" )")]
  [InlineData("@import url(x)y")]
  [InlineData("@import url('x')y")]
  [InlineData("@import url(\"x\")y")]
  public void CssImportRegex_ShouldMatch(string input)
  {
    const string expectedUrl = "x";

    var match = _cssImport.Match(input);

    Assert.True(match.Success);
    Assert.Equal(expectedUrl, match.Groups["url"].Value);
  }
}
