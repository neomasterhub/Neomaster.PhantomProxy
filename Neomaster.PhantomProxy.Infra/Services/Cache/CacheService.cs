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
  public RsaPems RestoreRsaPems(string key, TimeSpan? duration = null)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(key);

    if (memoryCache.TryGetValue(key, out RsaPems? pems)
      && pems != null)
    {
      return pems;
    }

    pems = RsaEncryptor.GeneratePems();
    memoryCache.Set(key, pems, duration ?? settings.EncryptionKeysLifetime);

    return pems;
  }
}
