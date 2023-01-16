using System.Text.Json;
using Buttercup.Api.TestUtils;
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
    public async void QueryingRecipe()
    {
        using var dbContext = await this.appFactory.DbContextFactory.CreateDbContextAsync();

        try
        {
            var recipe = this.sampleDataFactory.BuildRecipe(setOptionalAttributes: true);

            dbContext.Recipes.Add(recipe);

            await dbContext.SaveChangesAsync();

            using var client = this.appFactory.CreateClient();

            Task<JsonDocument> PostRecipeQuery(long id) => client.PostQuery(
                @"query($id: Long!) {
                    recipe(id: $id) {
                        id
                        title
                        contributor { id email }
                    }
                }",
                new { id });

            JsonElement GetRecipeElement(JsonDocument document) =>
                document.RootElement.GetProperty("data").GetProperty("recipe");

            using (var document = await PostRecipeQuery(recipe.Id))
            {
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

                JsonAssert.Equivalent(expected, GetRecipeElement(document));
            }

            using (var document = await PostRecipeQuery(this.sampleDataFactory.NextInt()))
            {
                JsonAssert.Null(GetRecipeElement(document));
            }
        }
        finally
        {
            await dbContext.Recipes.ExecuteDeleteAsync();
            await dbContext.Users.ExecuteDeleteAsync();
        }
    }

    [Fact]
    public async void QueryingRecipes()
    {
        using var dbContext = await this.appFactory.DbContextFactory.CreateDbContextAsync();

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
