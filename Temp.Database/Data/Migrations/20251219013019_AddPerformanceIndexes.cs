using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Temp.Database.Data.Migrations
{

    public partial class AddPerformanceIndexes : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "Employees",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                defaultValue: "None",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldDefaultValue: "None");

            migrationBuilder.AlterColumn<string>(
                name: "AppUserId",
                table: "Employees",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Workplaces_Name",
                table: "Workplaces",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_GroupId_IsActive",
                table: "Teams",
                columns: new[] { "GroupId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Teams_IsActive",
                table: "Teams",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_Name",
                table: "Teams",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_IsActive",
                table: "Organizations",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_IsActive_HasActiveGroup",
                table: "Organizations",
                columns: new[] { "IsActive", "HasActiveGroup" });

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_Name",
                table: "Organizations",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_IsActive",
                table: "Groups",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_Name",
                table: "Groups",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_OrganizationId_IsActive",
                table: "Groups",
                columns: new[] { "OrganizationId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Engagements_DateRange",
                table: "Engagements",
                columns: new[] { "DateFrom", "DateTo" });

            migrationBuilder.CreateIndex(
                name: "IX_Engagements_DateTo",
                table: "Engagements",
                column: "DateTo");

            migrationBuilder.CreateIndex(
                name: "IX_Engagements_EmployeeId_DateTo",
                table: "Engagements",
                columns: new[] { "EmployeeId", "DateTo" });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_AppUserId",
                table: "Employees",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Name",
                table: "Employees",
                columns: new[] { "FirstName", "LastName" });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Role",
                table: "Employees",
                column: "Role");
        }


        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropIndex(
                name: "IX_Workplaces_Name",
                table: "Workplaces");

            migrationBuilder.DropIndex(
                name: "IX_Teams_GroupId_IsActive",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_Teams_IsActive",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_Teams_Name",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_Organizations_IsActive",
                table: "Organizations");

            migrationBuilder.DropIndex(
                name: "IX_Organizations_IsActive_HasActiveGroup",
                table: "Organizations");

            migrationBuilder.DropIndex(
                name: "IX_Organizations_Name",
                table: "Organizations");

            migrationBuilder.DropIndex(
                name: "IX_Groups_IsActive",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_Name",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_OrganizationId_IsActive",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Engagements_DateRange",
                table: "Engagements");

            migrationBuilder.DropIndex(
                name: "IX_Engagements_DateTo",
                table: "Engagements");

            migrationBuilder.DropIndex(
                name: "IX_Engagements_EmployeeId_DateTo",
                table: "Engagements");

            migrationBuilder.DropIndex(
                name: "IX_Employees_AppUserId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_Name",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_Role",
                table: "Employees");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "None",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldDefaultValue: "None");

            migrationBuilder.AlterColumn<string>(
                name: "AppUserId",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}