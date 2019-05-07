using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WeStop.Infra.Migrations
{
    public partial class entity_gameroom_players_map : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                table: "theme",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("486facd3-8698-4fa7-b434-ad694552bf85"), "Nome" },
                    { new Guid("d1f7b77a-707d-451c-817a-d1134669df59"), "Sobrenome" },
                    { new Guid("d9f9fb67-08d7-4f77-bb04-0a86a4a954df"), "CEP" },
                    { new Guid("79469570-2978-468c-8b29-56651caf86e2"), "FDS" },
                    { new Guid("74a30b8c-6ae4-4c9f-9142-1344f976e728"), "Carro" },
                    { new Guid("f69cad87-25a0-4e77-8707-364afafe08c1"), "Marca" },
                    { new Guid("1a498682-7297-4ab4-9e3e-70edd84928b0"), "Objeto" },
                    { new Guid("b1a82dc5-58de-43da-b42e-409e36f88668"), "Cor" },
                    { new Guid("0354a186-6ee9-46f6-8a0b-8437d6993ae6"), "Fruta" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("0354a186-6ee9-46f6-8a0b-8437d6993ae6"));

            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("1a498682-7297-4ab4-9e3e-70edd84928b0"));

            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("486facd3-8698-4fa7-b434-ad694552bf85"));

            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("74a30b8c-6ae4-4c9f-9142-1344f976e728"));

            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("79469570-2978-468c-8b29-56651caf86e2"));

            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("b1a82dc5-58de-43da-b42e-409e36f88668"));

            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("d1f7b77a-707d-451c-817a-d1134669df59"));

            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("d9f9fb67-08d7-4f77-bb04-0a86a4a954df"));

            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("f69cad87-25a0-4e77-8707-364afafe08c1"));

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
    }
}
