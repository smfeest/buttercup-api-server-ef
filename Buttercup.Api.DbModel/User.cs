namespace Buttercup.Api.DbModel;

public class User
{
    public required long Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public string? PasswordHash { get; set; }
    public required string TimeZone { get; set; }
    public required DateTime Created { get; set; }
}
