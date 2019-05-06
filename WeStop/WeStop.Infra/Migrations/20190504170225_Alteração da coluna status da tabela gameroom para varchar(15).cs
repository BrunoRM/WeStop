using Microsoft.EntityFrameworkCore.Migrations;

namespace WeStop.Infra.Migrations
{
    public partial class Alteraçãodacolunastatusdatabelagameroomparavarchar15 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "status",
                schema: "public",
                table: "gameroom",
                type: "character varying(15)",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "status",
                schema: "public",
                table: "gameroom",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(15)");
        }
    }
}
