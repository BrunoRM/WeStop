using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WeStop.Infra.Migrations.WeStopNPgDb
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "gameroom",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    name = table.Column<string>(maxLength: 15, nullable: false),
                    password = table.Column<string>(type: "character varying(255)", nullable: false),
                    number_of_rounds = table.Column<int>(nullable: false),
                    number_of_players = table.Column<int>(nullable: false),
                    themes = table.Column<string>(maxLength: 120, nullable: false),
                    available_letters = table.Column<string>(maxLength: 52, nullable: false),
                    status = table.Column<string>(type: "character varying(15)", nullable: false),
                    created_at = table.Column<DateTime>(nullable: false),
                    expiraton_date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gameroom", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "player",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    name = table.Column<string>(maxLength: 35, nullable: false),
                    username = table.Column<string>(maxLength: 20, nullable: false),
                    email = table.Column<string>(maxLength: 50, nullable: false),
                    password = table.Column<string>(type: "character varying(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "theme",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    name = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_theme", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "gameroom_player",
                schema: "public",
                columns: table => new
                {
                    gameroom_id = table.Column<Guid>(nullable: false),
                    player_id = table.Column<Guid>(nullable: false),
                    is_admin = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gameroom_player", x => new { x.gameroom_id, x.player_id });
                    table.ForeignKey(
                        name: "FK_gameroom_player_gameroom_gameroom_id",
                        column: x => x.gameroom_id,
                        principalSchema: "public",
                        principalTable: "gameroom",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_gameroom_player_player_player_id",
                        column: x => x.player_id,
                        principalSchema: "public",
                        principalTable: "player",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "theme",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("b40ae9e5-9e16-4441-8a01-494db1052f50"), "Nome" },
                    { new Guid("6039ae57-0723-4b61-a028-37406f1187bc"), "Sobrenome" },
                    { new Guid("309a5da6-78e3-4d53-863d-06b952c47bee"), "CEP" },
                    { new Guid("3f54d595-160a-4e35-87e6-ba4b3ecf5683"), "FDS" },
                    { new Guid("f2fc7214-2f54-4963-9ce9-5f9bc800b73b"), "Carro" },
                    { new Guid("cd7ccfa6-082c-4473-8a92-ed2cfe2b0c3b"), "Marca" },
                    { new Guid("cc3ac47c-f5d5-4063-9eff-dd475cad059b"), "Objeto" },
                    { new Guid("92e873fa-1b76-413e-b969-c3c3d8e980fb"), "Cor" },
                    { new Guid("35024700-7892-4dd8-aaca-bef08735ea1f"), "Fruta" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_gameroom_player_player_id",
                schema: "public",
                table: "gameroom_player",
                column: "player_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "gameroom_player",
                schema: "public");

            migrationBuilder.DropTable(
                name: "theme",
                schema: "public");

            migrationBuilder.DropTable(
                name: "gameroom",
                schema: "public");

            migrationBuilder.DropTable(
                name: "player",
                schema: "public");
        }
    }
}
