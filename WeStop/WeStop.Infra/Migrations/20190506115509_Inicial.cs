using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WeStop.Infra.Migrations
{
    public partial class Inicial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "gameroom",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    name = table.Column<string>(maxLength: 15, nullable: false),
                    password = table.Column<string>(type: "varchar(255)", nullable: false),
                    number_of_rounds = table.Column<int>(nullable: false),
                    number_of_players = table.Column<int>(nullable: false),
                    status = table.Column<string>(type: "varchar(15)", nullable: false),
                    created_at = table.Column<DateTime>(nullable: false),
                    expiraton_date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gameroom", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "theme",
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
                name: "player",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    name = table.Column<string>(maxLength: 35, nullable: false),
                    username = table.Column<string>(maxLength: 20, nullable: false),
                    email = table.Column<string>(maxLength: 50, nullable: false),
                    password = table.Column<string>(type: "varchar(255)", nullable: false),
                    GameRoomId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player", x => x.id);
                    table.ForeignKey(
                        name: "FK_player_gameroom_GameRoomId",
                        column: x => x.GameRoomId,
                        principalTable: "gameroom",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "theme",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("181655f7-8649-4eed-a30e-af1d50897522"), "Nome" },
                    { new Guid("68183d37-9282-47ef-832f-3c6837bfc6e7"), "Sobrenome" },
                    { new Guid("47d5b7c2-748c-4d6f-b3f4-d3d807f563f5"), "CEP" },
                    { new Guid("1b113fe6-7365-43b6-845a-b62548a07c94"), "FDS" },
                    { new Guid("70e39d6d-7317-426f-a227-7d3d50230cbb"), "Carro" },
                    { new Guid("cacd0f82-c2ab-44a3-ac1b-0b23ec736177"), "Marca" },
                    { new Guid("87f3ce4a-de91-4425-a5ba-3a93bfa57c61"), "Objeto" },
                    { new Guid("c4f76812-eff0-452b-9e4a-7fbb6c05fad0"), "Cor" },
                    { new Guid("fe094679-bd10-4fdb-ac25-000aec2a4ae4"), "Fruta" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_player_GameRoomId",
                table: "player",
                column: "GameRoomId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "player");

            migrationBuilder.DropTable(
                name: "theme");

            migrationBuilder.DropTable(
                name: "gameroom");
        }
    }
}
