using Microsoft.EntityFrameworkCore.Migrations;

namespace Temp.Database.Migrations
{
    public partial class nullableModerator : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Moderators_ModeratorId",
                table: "Applications");

            migrationBuilder.AlterColumn<int>(
                name: "ModeratorId",
                table: "Applications",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Moderators_ModeratorId",
                table: "Applications",
                column: "ModeratorId",
                principalTable: "Moderators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Moderators_ModeratorId",
                table: "Applications");

            migrationBuilder.AlterColumn<int>(
                name: "ModeratorId",
                table: "Applications",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Moderators_ModeratorId",
                table: "Applications",
                column: "ModeratorId",
                principalTable: "Moderators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}