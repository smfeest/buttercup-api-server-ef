namespace Buttercup.Api.DbModel;

public class Session
{
    public required long Id { get; set; }
    public required string AccessToken { get; set; }
}
