using Autofac;
using Autofac.Extras.Moq;
using Moq;
using Neomaster.PhantomProxy.App;
using Neomaster.PhantomProxy.Common;

namespace Neomaster.PhantomProxy.Infra.UnitTests;

public class SessionServiceUnitTests
{
  private readonly ISessionService _sut;
  private readonly Mock<ICacheService> _mockCacheService;
  private readonly PhantomProxySettings _phantomProxySettings;

  public SessionServiceUnitTests()
  {
    _mockCacheService = new Mock<ICacheService>();

    _phantomProxySettings = new PhantomProxySettings
    {
      EncryptionKeysLifetime = TimeSpan.FromSeconds(1),
    };

    var autoMock = AutoMock.GetLoose(
      builder =>
      {
        builder.RegisterMock(_mockCacheService);
        builder.RegisterInstance(_phantomProxySettings);
        builder.RegisterType<SessionService>().As<ISessionService>();
      });

    _sut = autoMock.Create<ISessionService>();
  }

  [Fact]
  public void ShouldBeCreated()
  {
    Assert.NotNull(_sut);
    Assert.IsType<SessionService>(_sut);
  }

  [Fact]
  public void Start_ShouldReturnSessionInfo()
  {
    var sessionIdGuid = Guid.Empty;
    var pems = new RsaPems
    {
      PublicPem = "1",
      PrivatePem = "2",
    };
    _mockCacheService
      .Setup(m => m.RestoreRsaPems(It.Is<string?>(key => Guid.TryParse(key, out sessionIdGuid)), null))
      .Returns(pems);

    var sessionInfo = _sut.Start();

    Assert.NotNull(sessionInfo);
    Assert.Equal(pems.PublicPem, sessionInfo.Pem);
    Assert.Equal(_phantomProxySettings.EncryptionKeysLifetime, sessionInfo.Lifetime);
    Assert.Equal(sessionIdGuid.ToString(), sessionInfo.Id);
    _mockCacheService.Verify(v => v.RestoreRsaPems(It.Is<string?>(key => Guid.TryParse(key, out sessionIdGuid)), null), Times.Once());
  }
}
