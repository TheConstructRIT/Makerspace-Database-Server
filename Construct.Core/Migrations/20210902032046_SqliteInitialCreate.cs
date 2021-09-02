using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Construct.Core.Migrations
{
    public partial class SqliteInitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PrintMaterials",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    CostPerGram = table.Column<float>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrintMaterials", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    HashedId = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Permissions = table.Column<string>(type: "TEXT", nullable: true),
                    SignUpTime = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.HashedId);
                });

            migrationBuilder.CreateTable(
                name: "PrintLog",
                columns: table => new
                {
                    UserHashedId = table.Column<string>(type: "TEXT", nullable: false),
                    Time = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", nullable: false),
                    MaterialName = table.Column<string>(type: "TEXT", nullable: false),
                    WeightGrams = table.Column<float>(type: "REAL", nullable: false),
                    BillTo = table.Column<string>(type: "TEXT", nullable: true),
                    Cost = table.Column<float>(type: "REAL", nullable: false),
                    Owed = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_PrintLog_PrintMaterials_MaterialName",
                        column: x => x.MaterialName,
                        principalTable: "PrintMaterials",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrintLog_Users_UserHashedId",
                        column: x => x.UserHashedId,
                        principalTable: "Users",
                        principalColumn: "HashedId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VisitLogs",
                columns: table => new
                {
                    UserHashedId = table.Column<string>(type: "TEXT", nullable: false),
                    Source = table.Column<string>(type: "TEXT", nullable: false),
                    Time = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_VisitLogs_Users_UserHashedId",
                        column: x => x.UserHashedId,
                        principalTable: "Users",
                        principalColumn: "HashedId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrintLog_MaterialName",
                table: "PrintLog",
                column: "MaterialName");

            migrationBuilder.CreateIndex(
                name: "IX_PrintLog_UserHashedId",
                table: "PrintLog",
                column: "UserHashedId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitLogs_UserHashedId",
                table: "VisitLogs",
                column: "UserHashedId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrintLog");

            migrationBuilder.DropTable(
                name: "VisitLogs");

            migrationBuilder.DropTable(
                name: "PrintMaterials");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
