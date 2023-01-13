using Microsoft.Extensions.Logging;

namespace Buttercup.Api.TestUtils;

/// <summary>
/// A fake logger that stores log entries in a list.
/// </summary>
/// <typeparam name="TCategoryName">
/// The type whose name is used for the logger category name.
/// </typeparam>
public class ListLogger<TCategoryName> : ILogger<TCategoryName>
{
    private readonly List<LogEntry> entries = new();

    /// <summary>
    /// Gets the list of log entries.
    /// </summary>
    public IReadOnlyList<LogEntry> Entries => this.entries;

    /// <inheritdoc/>
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    /// <inheritdoc/>
    public bool IsEnabled(LogLevel logLevel) => true;

    /// <inheritdoc/>
    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter) =>
        this.entries.Add(new(logLevel, eventId, formatter(state, exception), state, exception));

    /// <summary>
    /// Represents a log entry.
    /// </summary>
    /// <param name="LogLevel">The log level.</param>
    /// <param name="EventId">The event ID.</param>
    /// <param name="Message">The formatted message.</param>
    /// <param name="State">The data.</param>
    /// <param name="Exception">The associated exception, if any.</param>
    public sealed record LogEntry(
        LogLevel LogLevel, EventId EventId, string Message, object? State, Exception? Exception);
}
