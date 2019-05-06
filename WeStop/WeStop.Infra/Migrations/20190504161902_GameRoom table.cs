using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WeStop.Infra.Migrations
{
    public partial class GameRoomtable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GameRoomId",
                schema: "public",
                table: "player",
                nullable: true);

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
                    status = table.Column<int>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: false),
                    expiraton_date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gameroom", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_player_GameRoomId",
                schema: "public",
                table: "player",
                column: "GameRoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_player_gameroom_GameRoomId",
                schema: "public",
                table: "player",
                column: "GameRoomId",
                principalSchema: "public",
                principalTable: "gameroom",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_player_gameroom_GameRoomId",
                schema: "public",
                table: "player");

            migrationBuilder.DropTable(
                name: "gameroom",
                schema: "public");

            migrationBuilder.DropIndex(
                name: "IX_player_GameRoomId",
                schema: "public",
                table: "player");

            migrationBuilder.DropColumn(
                name: "GameRoomId",
                schema: "public",
                table: "player");
        }
    }
}
