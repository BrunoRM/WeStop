using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WeStop.Infra.Migrations
{
    public partial class gameroom_player_table_is_admin_column : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<bool>(
                name: "is_admin",
                table: "gameroom_player",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "theme",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("4736950f-1e07-45e1-ab56-1f9cf93b7d77"), "Nome" },
                    { new Guid("ddad0d5f-8e18-42bc-bb6d-dd0729f7d675"), "Sobrenome" },
                    { new Guid("498f47af-fe9e-42d9-9c29-6a7025f377e7"), "CEP" },
                    { new Guid("cd81e591-3c65-490a-ad8a-c7161a53766c"), "FDS" },
                    { new Guid("abe33b77-6c06-48c1-8b03-ffe45d0a2a8e"), "Carro" },
                    { new Guid("f5f4860e-bbdf-404a-8a2d-0610eb3b2c50"), "Marca" },
                    { new Guid("ec2d21ca-3f99-4d2b-930e-29592902cd74"), "Objeto" },
                    { new Guid("7c85230d-4db7-482d-b960-3384f9d342e5"), "Cor" },
                    { new Guid("a6ca80e4-c130-4ec6-b106-7f4d0b75ea16"), "Fruta" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("4736950f-1e07-45e1-ab56-1f9cf93b7d77"));

            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("498f47af-fe9e-42d9-9c29-6a7025f377e7"));

            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("7c85230d-4db7-482d-b960-3384f9d342e5"));

            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("a6ca80e4-c130-4ec6-b106-7f4d0b75ea16"));

            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("abe33b77-6c06-48c1-8b03-ffe45d0a2a8e"));

            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("cd81e591-3c65-490a-ad8a-c7161a53766c"));

            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("ddad0d5f-8e18-42bc-bb6d-dd0729f7d675"));

            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("ec2d21ca-3f99-4d2b-930e-29592902cd74"));

            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("f5f4860e-bbdf-404a-8a2d-0610eb3b2c50"));

            migrationBuilder.DropColumn(
                name: "is_admin",
                table: "gameroom_player");

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
        }
    }
}
