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
      1
      <div srcset="a"></div>
      <div srcset=" a "></div>
      <div srcset="a 1x"></div>
      <div srcset=" a  1x "></div>

      2
      <div srcset="a,b"></div>
      <div srcset=" a , b "></div>

      3
      <div srcset="a 1x,b"></div>
      <div srcset=" a  1x , b "></div>

      4
      <div srcset="a,b 1w"></div>
      <div srcset=" a , b  1w "></div>

      5
      <div srcset="a 1x,b 1w"></div>
      <div srcset=" a  1x , b  1w "></div>

      6
      <div srcset="
       a
        1x
         ,
          b
           1w
            "></div>
      """);
    var expected = new SrcsetValueItem[]
    {
      // 1
      new() { Url = "a" },
      new() { Url = "a" },
      new() { Url = "a", Descriptor = "1x" },
      new() { Url = "a", Descriptor = "1x" },

      // 2
      new() { Url = "a" }, new() { Url = "b" },
      new() { Url = "a" }, new() { Url = "b" },

      // 3
      new() { Url = "a", Descriptor = "1x" }, new() { Url = "b" },
      new() { Url = "a", Descriptor = "1x" }, new() { Url = "b" },

      // 4
      new() { Url = "a" }, new() { Url = "b", Descriptor = "1w" },
      new() { Url = "a" }, new() { Url = "b", Descriptor = "1w" },

      // 5
      new() { Url = "a", Descriptor = "1x" }, new() { Url = "b", Descriptor = "1w" },
      new() { Url = "a", Descriptor = "1x" }, new() { Url = "b", Descriptor = "1w" },

      // 6
      new() { Url = "a", Descriptor = "1x" }, new() { Url = "b", Descriptor = "1w" },
    };

    var actual = doc.GetAllSrcsetValueItems().ToArray();

    Assert.Equal(expected.Length, actual.Length);
    Assert.All(actual, (a, i) =>
    {
      Assert.Equal(a, expected[i]);
    });
  }

  [Fact]
  public void GetAllSrcsetValueItems_ShouldReturnAllValueItems_WhenInvalidValues()
  {
    const string c = ",, ,";
    var doc = new HtmlDocument();
    doc.LoadHtml(
      $"""
      <div srcset="{c}a           {c}"></div>
      <div srcset="{c}a 1x        {c}"></div>
      <div srcset="{c}a   {c} b   {c}"></div>
      <div srcset="{c}a 1x{c} b   {c}"></div>
      <div srcset="{c}a   {c} b 1w{c}"></div>
      <div srcset="{c}a 1x{c} b 1w{c}"></div>
      """);
    var expected = new SrcsetValueItem[]
    {
      new() { Url = "a" },
      new() { Url = "a", Descriptor = "1x" },
      new() { Url = "a" }, new() { Url = "b" },
      new() { Url = "a", Descriptor = "1x" }, new() { Url = "b" },
      new() { Url = "a" }, new() { Url = "b", Descriptor = "1w" },
      new() { Url = "a", Descriptor = "1x" }, new() { Url = "b", Descriptor = "1w" },
    };

    var actual = doc.GetAllSrcsetValueItems().ToArray();

    Assert.Equal(expected.Length, actual.Length);
    Assert.All(actual, (a, i) =>
    {
      Assert.Equal(a, expected[i]);
    });
  }
}
