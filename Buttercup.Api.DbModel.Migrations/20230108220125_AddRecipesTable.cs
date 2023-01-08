using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Buttercup.Api.DbModel.Migrations;

public partial class AddRecipesTable : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "recipes",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                title = table.Column<string>(type: "text", nullable: false),
                preparation_minutes = table.Column<int>(type: "integer", nullable: true),
                cooking_minutes = table.Column<int>(type: "integer", nullable: true),
                servings = table.Column<int>(type: "integer", nullable: true),
                ingredients = table.Column<List<string>>(type: "text[]", nullable: false),
                steps = table.Column<List<string>>(type: "text[]", nullable: false),
                source = table.Column<string>(type: "text", nullable: true),
                created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                contributor_id = table.Column<long>(type: "bigint", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_recipes", x => x.id);
                table.ForeignKey(
                    name: "fk_recipes_users_contributor_id",
                    column: x => x.contributor_id,
                    principalTable: "users",
                    principalColumn: "id");
            });

        migrationBuilder.CreateIndex(
            name: "ix_recipes_contributor_id",
            table: "recipes",
            column: "contributor_id");
    }

    protected override void Down(MigrationBuilder migrationBuilder) =>
        migrationBuilder.DropTable(name: "recipes");
}
