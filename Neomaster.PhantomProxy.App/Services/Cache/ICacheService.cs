using Neomaster.PhantomProxy.Common;

namespace Neomaster.PhantomProxy.App;

/// <summary>
/// Service for managing cache operations.
/// </summary>
public interface ICacheService
{
  /// <summary>
  /// Restores RSA PEM-encoded keys from cache or generates new if missing and saves them.
  /// </summary>
  /// <param name="key">Cache key.</param>
  /// <param name="duration">Cache duration.</param>
  /// <returns>RSA PEM-encoded keys.</returns>
  RsaPems RestoreRsaPems(string key, TimeSpan? duration = null);
}
