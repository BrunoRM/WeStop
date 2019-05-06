using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WeStop.Infra.Migrations
{
    public partial class Seed_temas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "public",
                table: "theme",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("395cc6f2-3018-4a7c-90f3-369087faacbb"), "Nome" },
                    { new Guid("695cf63d-2760-4b1d-b8f7-39879bd4ee29"), "Sobrenome" },
                    { new Guid("03911ab8-5e9c-4f19-9e0d-d94c5d87ab3e"), "CEP" },
                    { new Guid("0402a910-fd0c-4de0-a871-17ccc93640ca"), "FDS" },
                    { new Guid("210ec31e-db11-485d-b4a7-fb15d0d8721a"), "Carro" },
                    { new Guid("5208928f-47ed-44ff-8ee7-e2c32574a53e"), "Marca" },
                    { new Guid("7015fdbb-3179-450f-9c7f-bac658957812"), "Objeto" },
                    { new Guid("9114719c-64bb-46a5-879f-9c3fb3d8e674"), "Cor" },
                    { new Guid("05a69df1-a1eb-441b-85a8-00d5231fb034"), "Fruta" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "public",
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("03911ab8-5e9c-4f19-9e0d-d94c5d87ab3e"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("0402a910-fd0c-4de0-a871-17ccc93640ca"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("05a69df1-a1eb-441b-85a8-00d5231fb034"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("210ec31e-db11-485d-b4a7-fb15d0d8721a"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("395cc6f2-3018-4a7c-90f3-369087faacbb"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("5208928f-47ed-44ff-8ee7-e2c32574a53e"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("695cf63d-2760-4b1d-b8f7-39879bd4ee29"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("7015fdbb-3179-450f-9c7f-bac658957812"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "theme",
                keyColumn: "id",
                keyValue: new Guid("9114719c-64bb-46a5-879f-9c3fb3d8e674"));
        }
    }
}
