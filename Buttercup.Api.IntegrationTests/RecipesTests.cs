using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Buttercup.Api.IntegrationTests;

[Collection(nameof(IntegrationTestCollection))]
public class RecipesTests
{
    private readonly AppFactory appFactory;
    private readonly SampleDataFactory sampleDataFactory = new();

    public RecipesTests(AppFactory appFactory) => this.appFactory = appFactory;

    [Fact]
    public async void QueryingRecipes()
    {
        using var dbContext = await this.appFactory.CreateAppDbContext();

        try
        {
            var recipe = this.sampleDataFactory.BuildRecipe(setOptionalAttributes: true);

            dbContext.Recipes.Add(recipe);

            await dbContext.SaveChangesAsync();

            using var client = this.appFactory.CreateClient();

            using var document = await client.PostQuery(
                @"{
                    recipes {
                        id
                        title
                        contributor { id email }
                    }
                }");

            var expected = new
            {
                recipe.Id,
                recipe.Title,
                Contributor = new
                {
                    recipe.Contributor!.Id,
                    recipe.Contributor.Email
                }
            };

            JsonAssert.Equivalent(
                new[] { expected },
                document.RootElement.GetProperty("data").GetProperty("recipes"));
        }
        finally
        {
            await dbContext.Recipes.ExecuteDeleteAsync();
            await dbContext.Users.ExecuteDeleteAsync();
        }
    }
}
