using Microsoft.EntityFrameworkCore.Migrations;

namespace XipeADNWeb.Migrations
{
    public partial class orlixborlix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Opportunities_AspNetUsers_UserId1",
                table: "Opportunities");

            migrationBuilder.DropIndex(
                name: "IX_Opportunities_UserId1",
                table: "Opportunities");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Opportunities");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Opportunities",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<string>(
                name: "About",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkedIn",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Naos",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Twitter",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Opportunities_UserId",
                table: "Opportunities",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Opportunities_AspNetUsers_UserId",
                table: "Opportunities",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Opportunities_AspNetUsers_UserId",
                table: "Opportunities");

            migrationBuilder.DropIndex(
                name: "IX_Opportunities_UserId",
                table: "Opportunities");

            migrationBuilder.DropColumn(
                name: "About",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LinkedIn",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Naos",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Twitter",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Opportunities",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "Opportunities",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Opportunities_UserId1",
                table: "Opportunities",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Opportunities_AspNetUsers_UserId1",
                table: "Opportunities",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
