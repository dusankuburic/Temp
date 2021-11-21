using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Temp.Database.Migrations
{
    public partial class AddedApplication : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    TeamId = table.Column<int>(nullable: false),
                    ModeratorId = table.Column<int>(nullable: false),
                    Category = table.Column<string>(nullable: true),
                    Content = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Status = table.Column<bool>(nullable: false),
                    StatusUpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Applications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Applications_Moderators_ModeratorId",
                        column: x => x.ModeratorId,
                        principalTable: "Moderators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Applications_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Applications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ModeratorId",
                table: "Applications",
                column: "ModeratorId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_TeamId",
                table: "Applications",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_UserId",
                table: "Applications",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(
                name: "Applications");
        }
    }
}