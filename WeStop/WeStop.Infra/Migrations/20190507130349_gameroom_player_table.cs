using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WeStop.Infra.Migrations
{
    public partial class gameroom_player_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_player_gameroom_GameRoomId",
                table: "player");

            migrationBuilder.DropIndex(
                name: "IX_player_GameRoomId",
                table: "player");

            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("016dc905-1375-4f55-9083-628fb088c4c3"));

            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("285caca1-52ba-465e-b944-e898e7cf9804"));

            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("46182272-21e1-48f2-86ce-96fb1e8ab305"));

            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("689d8d45-dc15-4ce3-a389-9e30231b7667"));

            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("7bbb49ca-6af3-4bff-b8de-6d00ebaadf7d"));

            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("9c67ebb2-cda2-44ec-854e-f28812ab0d93"));

            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("bf2e7d6f-ff11-4f78-a3e3-cbf385a15ef1"));

            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("cecbd6df-c0a7-4d39-9174-9c854c7e806c"));

            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("fef19edc-6146-4816-a643-50d331ec4e76"));

            migrationBuilder.DropColumn(
                name: "GameRoomId",
                table: "player");

            migrationBuilder.CreateTable(
                name: "gameroom_player",
                columns: table => new
                {
                    gameroom_id = table.Column<Guid>(nullable: false),
                    player_id = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gameroom_player", x => new { x.gameroom_id, x.player_id });
                    table.ForeignKey(
                        name: "FK_gameroom_player_gameroom_gameroom_id",
                        column: x => x.gameroom_id,
                        principalTable: "gameroom",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_gameroom_player_player_player_id",
                        column: x => x.player_id,
                        principalTable: "player",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "theme",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("ce9efc4d-0302-4f15-913d-b3a7082aed3e"), "Nome" },
                    { new Guid("2889c013-cf0f-4789-84a8-64bb35ad3806"), "Sobrenome" },
                    { new Guid("f19aac0c-a945-48e4-a132-53a7366861bb"), "CEP" },
                    { new Guid("dcc57e28-9240-473b-8788-82743b404050"), "FDS" },
                    { new Guid("4c27fc4e-758b-4722-ac67-2899462cb888"), "Carro" },
                    { new Guid("170b4872-9145-48e1-bc18-502fd1e9c4fc"), "Marca" },
                    { new Guid("8e4cd5ff-330f-44e5-8ade-ba70644422ef"), "Objeto" },
                    { new Guid("b3525d81-d94e-4429-9b75-33717dbb3b68"), "Cor" },
                    { new Guid("5f21b1b2-6213-4560-bcf3-42b31fe45bc0"), "Fruta" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_gameroom_player_player_id",
                table: "gameroom_player",
                column: "player_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "gameroom_player");

            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("170b4872-9145-48e1-bc18-502fd1e9c4fc"));

            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("2889c013-cf0f-4789-84a8-64bb35ad3806"));

            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("4c27fc4e-758b-4722-ac67-2899462cb888"));

            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("5f21b1b2-6213-4560-bcf3-42b31fe45bc0"));

            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("8e4cd5ff-330f-44e5-8ade-ba70644422ef"));

            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("b3525d81-d94e-4429-9b75-33717dbb3b68"));

            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("ce9efc4d-0302-4f15-913d-b3a7082aed3e"));

            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("dcc57e28-9240-473b-8788-82743b404050"));

            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("f19aac0c-a945-48e4-a132-53a7366861bb"));

            migrationBuilder.AddColumn<Guid>(
                name: "GameRoomId",
                table: "player",
                nullable: true);

            migrationBuilder.InsertData(
                table: "theme",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("016dc905-1375-4f55-9083-628fb088c4c3"), "Nome" },
                    { new Guid("285caca1-52ba-465e-b944-e898e7cf9804"), "Sobrenome" },
                    { new Guid("46182272-21e1-48f2-86ce-96fb1e8ab305"), "CEP" },
                    { new Guid("fef19edc-6146-4816-a643-50d331ec4e76"), "FDS" },
                    { new Guid("7bbb49ca-6af3-4bff-b8de-6d00ebaadf7d"), "Carro" },
                    { new Guid("689d8d45-dc15-4ce3-a389-9e30231b7667"), "Marca" },
                    { new Guid("cecbd6df-c0a7-4d39-9174-9c854c7e806c"), "Objeto" },
                    { new Guid("9c67ebb2-cda2-44ec-854e-f28812ab0d93"), "Cor" },
                    { new Guid("bf2e7d6f-ff11-4f78-a3e3-cbf385a15ef1"), "Fruta" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_player_GameRoomId",
                table: "player",
                column: "GameRoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_player_gameroom_GameRoomId",
                table: "player",
                column: "GameRoomId",
                principalTable: "gameroom",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
