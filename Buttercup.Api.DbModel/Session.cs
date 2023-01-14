namespace Buttercup.Api.DbModel;

/// <summary>
/// Represents a session.
/// </summary>
public class Session
{
    /// <summary>
    /// Gets or sets the primary key of the session.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the user.
    /// </summary>
    public required User User { get; set; }

    /// <summary>
    /// Gets or sets the date and time at which the session was created.
    /// </summary>
    public required DateTime Created { get; set; }

    /// <summary>
    /// Gets or sets the date and time at which the session was terminated.
    /// </summary>
    public DateTime? Terminated { get; set; }

    /// <summary>
    /// Gets or sets the date and time at which the current tokens were issued.
    /// </summary>
    public required DateTime CurrentTokensIssued { get; set; }

    /// <summary>
    /// Gets or sets the current token generation.
    /// </summary>
    public int CurrentTokenGeneration { get; set; }

    /// <summary>
    /// Gets or sets the generation of the most recently accepted token.
    /// </summary>
    public int? LastTokenGeneration { get; set; }
}
