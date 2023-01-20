namespace Buttercup.Api.DbModel;

public record Recipe
{
    public required long Id { get; set; }
    public required string Title { get; set; }
    public int? PreparationMinutes { get; set; }
    public int? CookingMinutes { get; set; }
    public int? Servings { get; set; }
    public required List<string> Ingredients { get; set; }
    public required List<string> Steps { get; set; }
    public string? Source { get; set; }
    public required DateTime Created { get; set; }
    public User? Contributor { get; set; }
}
