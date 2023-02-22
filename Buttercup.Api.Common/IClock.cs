namespace Buttercup.Api;

/// <summary>
/// Represents a service that can be used to get the current time.
/// </summary>
public interface IClock
{
    /// <summary>
    /// Gets the current time in Coordinated Universal Time (UTC).
    /// </summary>
    DateTime UtcNow { get; }
}
