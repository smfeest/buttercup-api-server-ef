namespace Buttercup.Api.DbModel;

/// <summary>
/// Represents a user.
/// </summary>
public record User
{
    /// <summary>
    /// Gets or sets the primary key of the user.
    /// </summary>
    public required long Id { get; set; }

    /// <summary>
    /// Gets or sets the user's name.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Gets or sets the user's hashed password, or null if the user does not have a password set.
    /// </summary>
    public string? PasswordHash { get; set; }

    /// <summary>
    /// Gets or sets the user's time zone as a TZID (e.g. 'Europe/London').
    /// </summary>
    public required string TimeZone { get; set; }

    /// <summary>
    /// Gets or sets the date and time at which the user was created.
    /// </summary>
    public required DateTime Created { get; set; }
}
