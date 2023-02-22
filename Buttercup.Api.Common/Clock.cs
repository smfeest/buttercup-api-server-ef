namespace Buttercup.Api;

/// <summary>
/// The default implementation of <see cref="IClock" />.
/// </summary>
public sealed class Clock : IClock
{
    /// <inheritdoc/>
    public DateTime UtcNow => DateTime.UtcNow;
}
