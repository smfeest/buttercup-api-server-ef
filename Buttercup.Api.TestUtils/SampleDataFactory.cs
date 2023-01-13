using Buttercup.Api.DbModel;

namespace Buttercup.Api.TestUtils;

public sealed class SampleDataFactory
{
    private int nextInt = 1;

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

    public User BuildUser(bool setOptionalAttributes = false) => new()
    {
        Id = this.NextInt(),
        Name = this.NextString("name"),
        Email = $"email-{this.NextInt()}@example.com",
        PasswordHash = setOptionalAttributes ? this.NextString("password-hash") : null,
        TimeZone = this.NextString("time-zone"),
        Created = this.NextDateTime()
    };

    public DateTime NextDateTime() =>
        new DateTime(2000, 1, 2, 3, 4, 5, DateTimeKind.Utc)
            + (new TimeSpan(1, 2, 3, 4, 5) * this.NextInt());

    public int NextInt() => this.nextInt++;

    public string NextString(string prefix) => $"{prefix}-{this.NextInt()}";
}
