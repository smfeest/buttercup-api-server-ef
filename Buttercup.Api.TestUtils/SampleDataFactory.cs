using Buttercup.Api.DbModel;

namespace Buttercup.Api.TestUtils;

/// <summary>
/// Provides methods for generating sample data.
/// </summary>
public sealed class SampleDataFactory
{
    private int nextInt = 1;

    /// <summary>
    /// Creates a new <see cref="Recipe" /> object with unique property values.
    /// </summary>
    /// <param name="setOptionalAttributes">
    /// <c>true</c> if optional properties should be populated; <c>false</c> if they should be left
    /// null.
    /// </param>
    /// <returns>The new <see cref="Recipe" /> object.</returns>
    public Recipe BuildRecipe(bool setOptionalAttributes = false) => new()
    {
        Id = this.NextInt(),
        Title = this.NextString("title"),
        PreparationMinutes = setOptionalAttributes ? this.NextInt() : null,
        CookingMinutes = setOptionalAttributes ? this.NextInt() : null,
        Servings = setOptionalAttributes ? this.NextInt() : null,
        Ingredients = new() { this.NextString("ingredient") },
        Steps = new() { this.NextString("step") },
        Source = setOptionalAttributes ? this.NextString("source") : null,
        Created = this.NextDateTime(),
        Contributor = setOptionalAttributes ? this.BuildUser(true) : null
    };


    /// <summary>
    /// Creates a new <see cref="User" /> object with unique property values.
    /// </summary>
    /// <param name="setOptionalAttributes">
    /// <c>true</c> if optional properties should be populated; <c>false</c> if they should be left
    /// null.
    /// </param>
    /// <returns>The new <see cref="User" /> object.</returns>
    public User BuildUser(bool setOptionalAttributes = false) => new()
    {
        Id = this.NextInt(),
        Name = this.NextString("name"),
        Email = $"email-{this.NextInt()}@example.com",
        PasswordHash = setOptionalAttributes ? this.NextString("password-hash") : null,
        TimeZone = this.NextString("time-zone"),
        Created = this.NextDateTime()
    };

    /// <summary>
    /// Generates a unique UTC date and time.
    /// </summary>
    /// <returns>The generated date and time.</returns>
    public DateTime NextDateTime() =>
        new DateTime(2000, 1, 2, 3, 4, 5, DateTimeKind.Utc)
            + (new TimeSpan(1, 2, 3, 4, 5) * this.NextInt());

    /// <summary>
    /// Generates a unique integer value.
    /// </summary>
    /// <returns>The generated integer value.</returns>
    public int NextInt() => this.nextInt++;

    /// <summary>
    /// Generates a unique string value.
    /// </summary>
    /// <param name="prefix">The prefix to be included in the string.</param>
    /// <returns>The generated string value.</returns>
    public string NextString(string prefix) => $"{prefix}-{this.NextInt()}";
}
