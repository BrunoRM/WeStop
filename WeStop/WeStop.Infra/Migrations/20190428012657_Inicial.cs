using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WeStop.Infra.Migrations
{
    public partial class Inicial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "player",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    name = table.Column<string>(maxLength: 35, nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    email = table.Column<string>(maxLength: 50, nullable: false),
                    password = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "player",
                schema: "public");
        }
    }
}
