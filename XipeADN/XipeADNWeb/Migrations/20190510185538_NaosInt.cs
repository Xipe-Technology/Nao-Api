using Microsoft.EntityFrameworkCore.Migrations;

namespace XipeADNWeb.Migrations
{
    public partial class NaosInt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Naos",
                table: "AspNetUsers",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Naos",
                table: "AspNetUsers",
                nullable: true,
                oldClrType: typeof(int));
        }
    }
}
