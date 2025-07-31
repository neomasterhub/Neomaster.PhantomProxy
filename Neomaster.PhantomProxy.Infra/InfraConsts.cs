namespace Neomaster.PhantomProxy.Infra;

/// <summary>
/// Infrastructure-level constants.
/// </summary>
public class InfraConsts
{
  /// <summary>
  /// URL prefixes skipped during proxying.
  /// </summary>
  public static readonly string[] IgnoredUrlPrefixes =
    [
      "#",
      "about:",
      "data:",
      "javascript:",
      "mailto:",
      "tel:",
    ];

  /// <summary>
  /// Common infrastructure error messages.
  /// </summary>
  public class ErrorMessages
  {
    public const string UrlEmpty = "URL cannot be empty.";
    public const string UrlInvalidFormat = "Invalid URL format.";
  }
}
