using System.Net.Http.Headers;
using System.Net.Mime;

namespace Neomaster.PhantomProxy.Infra.UnitTests;

public class HttpResponseMessageExtensionsUnitTests
{
  [Fact]
  public void GetContentType_ShouldReturnContentTypeFromResponse_WhenHeaderIsPresent()
  {
    const string expectedContentType = MediaTypeNames.Application.Xml;
    const string requestUrl = "1";
    var responseMessage = new HttpResponseMessage();
    responseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(expectedContentType);

    var actualContentType = responseMessage.GetContentType(requestUrl);

    Assert.Equal(expectedContentType, actualContentType);
  }

  [Fact]
  public void GetContentType_ShouldReturnContentTypeFromUrlExtension_WhenHeaderIsNotPresent()
  {
    const string expectedContentType = MediaTypeNames.Image.Png;
    const string requestUrl = "1.png";
    var responseMessage = new HttpResponseMessage();

    var actualContentType = responseMessage.GetContentType(requestUrl);

    Assert.Equal(expectedContentType, actualContentType);
  }
}
