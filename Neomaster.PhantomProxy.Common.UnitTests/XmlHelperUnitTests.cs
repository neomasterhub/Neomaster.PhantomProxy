using HtmlAgilityPack;

namespace Neomaster.PhantomProxy.Common.UnitTests;

public class XmlHelperUnitTests
{
  [Theory]
  [InlineData("a")]
  [InlineData("A")]
  public void GetHtmlAttributes_ShouldReturnAttribute(string findName)
  {
    var doc = new HtmlDocument();
    doc.LoadHtml("<div a:a></div>");

    var attributes = doc.GetHtmlAttributes([findName]);

    Assert.NotEmpty(attributes);
  }

  [Fact]
  public void GetAllSrcsetValueItems_ShouldReturnAllValueItems_WhenValidValues()
  {
    var doc = new HtmlDocument();
    doc.LoadHtml(
       """
      <div srcset="a"></div>
      <div srcset=" a "></div>
      <div srcset="a 1x"></div>
      <div srcset=" a 1x "></div>
      """);
    var expected = new SrcsetValueItem[]
    {
      new() { Url = "a" },
      new() { Url = "a" },
      new() { Url = "a", Descriptor = "1x" },
      new() { Url = "a", Descriptor = "1x" },
    };

    var actual = doc.GetAllSrcsetValueItems().ToArray();

    Assert.Equal(expected.Length, actual.Length);
    Assert.All(actual, (a, i) =>
    {
      Assert.Equal(a, expected[i]);
    });
  }
}
