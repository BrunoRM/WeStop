using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WeStop.Infra.Migrations
{
    public partial class gameroom_themes_and_available_letters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("181655f7-8649-4eed-a30e-af1d50897522"));

            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("1b113fe6-7365-43b6-845a-b62548a07c94"));

            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("47d5b7c2-748c-4d6f-b3f4-d3d807f563f5"));

            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("68183d37-9282-47ef-832f-3c6837bfc6e7"));

            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("70e39d6d-7317-426f-a227-7d3d50230cbb"));

            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("87f3ce4a-de91-4425-a5ba-3a93bfa57c61"));

            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("c4f76812-eff0-452b-9e4a-7fbb6c05fad0"));

            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("cacd0f82-c2ab-44a3-ac1b-0b23ec736177"));

            migrationBuilder.DeleteData(
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("fe094679-bd10-4fdb-ac25-000aec2a4ae4"));

            migrationBuilder.AddColumn<string>(
                name: "available_letters",
                table: "gameroom",
                maxLength: 52,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "themes",
                table: "gameroom",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "available_letters",
                table: "gameroom");

            migrationBuilder.DropColumn(
                name: "themes",
                table: "gameroom");

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
        }
    }
}
