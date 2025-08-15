namespace Neomaster.PhantomProxy.App;

/// <summary>
/// Service for managing sessions.
/// </summary>
public interface ISessionService
{
  /// <summary>
  /// Starts new session and returns session information.
  /// </summary>
  /// <returns>Session information.</returns>
  SessionInfo Start();
}
