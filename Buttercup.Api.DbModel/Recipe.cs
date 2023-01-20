namespace Buttercup.Api.DbModel;

/// <summary>
/// Represents a recipe.
/// </summary>
public record Recipe
{
    /// <summary>
    /// Gets or sets the primary key of the recipe.
    /// </summary>
    public required long Id { get; set; }

    /// <summary>
    /// Gets or sets the recipe title.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Gets or sets the preparation time in minutes.
    /// </summary>
    public int? PreparationMinutes { get; set; }

    /// <summary>
    /// Gets or sets the cooking time in minutes.
    /// </summary>
    public int? CookingMinutes { get; set; }

    /// <summary>
    /// Gets or sets the number of servings.
    /// </summary>
    public int? Servings { get; set; }

    /// <summary>
    /// Gets or sets the ingredients.
    /// </summary>
    public required List<string> Ingredients { get; set; }

    /// <summary>
    /// Gets or sets the steps in the method.
    /// </summary>
    public required List<string> Steps { get; set; }

    /// <summary>
    /// Gets or sets the source of the recipe.
    /// </summary>
    public string? Source { get; set; }

    /// <summary>
    /// Gets or sets the date and time at which the recipe was created.
    /// </summary>
    public required DateTime Created { get; set; }

    /// <summary>
    /// Gets or sets the user that created the recipe.
    /// </summary>
    public User? Contributor { get; set; }
}
