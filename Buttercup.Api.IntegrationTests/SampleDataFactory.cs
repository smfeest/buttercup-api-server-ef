using Buttercup.Api.DbModel;

namespace Buttercup.Api.IntegrationTests;

public sealed class SampleDataFactory
{
    private int nextInt = 1;

    public User BuildUser(bool setOptionalAttributes = false) => new()
    {
        Id = this.NextInt(),
        Name = this.NextString("name"),
        Email = $"email-{this.NextInt()}@example.com",
        PasswordHash = setOptionalAttributes ? this.NextString("password-hash") : null,
        TimeZone = this.NextString("time-zone"),
        Created = this.NextDateTime()
    };

    private DateTime NextDateTime() =>
        new DateTime(2000, 1, 2, 3, 4, 5, DateTimeKind.Utc)
            + (new TimeSpan(1, 2, 3, 4, 5) * this.NextInt());

    private int NextInt() => this.nextInt++;

    private string NextString(string prefix) => $"{prefix}-{this.NextInt()}";
}
