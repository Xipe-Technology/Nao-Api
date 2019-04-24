using Microsoft.EntityFrameworkCore.Migrations;

namespace XipeADNWeb.Data.Migrations
{
    public partial class addedpicurl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfilePicUrl",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePicUrl",
                table: "AspNetUsers");
        }
    }
}
