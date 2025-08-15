using Neomaster.PhantomProxy.App;

namespace Neomaster.PhantomProxy.Infra;

/// <inheritdoc/>
public class SessionService(
  PhantomProxySettings settings,
  ICacheService cacheService)
  : ISessionService
{
  /// <inheritdoc/>
  public SessionInfo Start()
  {
    var pems = cacheService.RestoreRsaPems();

    var sessionInfo = new SessionInfo
    {
      Pem = pems.PublicPem,
      Lifetime = settings.EncryptionKeysLifetime,
    };

    return sessionInfo;
  }
}
