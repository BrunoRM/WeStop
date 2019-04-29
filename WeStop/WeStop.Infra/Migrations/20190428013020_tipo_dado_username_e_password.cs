using Microsoft.EntityFrameworkCore.Migrations;

namespace WeStop.Infra.Migrations
{
    public partial class tipo_dado_username_e_password : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserName",
                schema: "public",
                table: "player",
                newName: "username");

            migrationBuilder.AlterColumn<string>(
                name: "username",
                schema: "public",
                table: "player",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "password",
                schema: "public",
                table: "player",
                type: "character varying(255)",
                nullable: false,
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "username",
                schema: "public",
                table: "player",
                newName: "UserName");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                schema: "public",
                table: "player",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "password",
                schema: "public",
                table: "player",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)");
        }
    }
}
