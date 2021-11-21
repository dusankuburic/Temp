using Microsoft.EntityFrameworkCore.Migrations;

namespace Temp.Database.Migrations
{
    public partial class EngagementSalaryAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.AddColumn<int>(
                name: "Salary",
                table: "Engagements",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropColumn(
                name: "Salary",
                table: "Engagements");
        }
    }
}