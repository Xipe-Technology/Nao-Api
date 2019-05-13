using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace XipeADNWeb.Migrations
{
    public partial class DateTime2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "MessageDateTime2",
                table: "Message",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MessageDateTime2",
                table: "Message");
        }
    }
}
