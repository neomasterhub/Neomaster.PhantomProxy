using Microsoft.Extensions.Caching.Memory;
using Neomaster.PhantomProxy.App;
using Neomaster.PhantomProxy.Common;

namespace Neomaster.PhantomProxy.Infra;

/// <inheritdoc/>
public class CacheService(
  IMemoryCache memoryCache,
  PhantomProxySettings settings)
  : ICacheService
{
  /// <inheritdoc/>
  public RsaPems RestoreRsaPems(string? key = null, TimeSpan? duration = null)
  {
    if (key != null
      && memoryCache.TryGetValue(key, out RsaPems? pems)
      && pems != null)
    {
      return pems;
    }

    pems = RsaEncryptor.GeneratePems();
    memoryCache.Set(pems.PublicPem, pems, duration ?? settings.EncryptionKeysLifetime);

    return pems;
  }
}
