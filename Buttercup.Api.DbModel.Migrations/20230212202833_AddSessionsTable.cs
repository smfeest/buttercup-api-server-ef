using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Buttercup.Api.DbModel.Migrations;

public partial class AddSessionsTable : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "sessions",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                user_id = table.Column<long>(type: "bigint", nullable: false),
                created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                terminated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                current_tokens_issued = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                current_token_generation = table.Column<int>(type: "integer", nullable: false),
                last_token_generation = table.Column<int>(type: "integer", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_sessions", x => x.id);
                table.ForeignKey(
                    name: "fk_sessions_users_user_id",
                    column: x => x.user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_sessions_user_id",
            table: "sessions",
            column: "user_id");
    }

    protected override void Down(MigrationBuilder migrationBuilder) =>
        migrationBuilder.DropTable(name: "sessions");
}
