using Microsoft.EntityFrameworkCore.Migrations;

namespace XipeADNWeb.Migrations
{
    public partial class nuevasTablas2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KPIs_Opportunities_OpportunityId",
                table: "KPIs");

            migrationBuilder.AlterColumn<int>(
                name: "OpportunityId",
                table: "KPIs",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_KPIs_Opportunities_OpportunityId",
                table: "KPIs",
                column: "OpportunityId",
                principalTable: "Opportunities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KPIs_Opportunities_OpportunityId",
                table: "KPIs");

            migrationBuilder.AlterColumn<int>(
                name: "OpportunityId",
                table: "KPIs",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_KPIs_Opportunities_OpportunityId",
                table: "KPIs",
                column: "OpportunityId",
                principalTable: "Opportunities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
