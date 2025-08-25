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
    var id = Guid.NewGuid().ToString();
    var pems = cacheService.RestoreRsaPems(id);

    var sessionInfo = new SessionInfo
    {
      Id = id,
      Pem = pems.PublicPem,
      Lifetime = settings.EncryptionKeysLifetime,
      OpenFrameLinkViaForm = settings.OpenFrameLinkViaForm,
    };

    return sessionInfo;
  }
}
