using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Construct.Core.Migrations.Postgres
{
    [ExcludeFromCodeCoverage]
    public partial class PostgresInitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PrintMaterials",
                columns: table => new
                {
                    Name = table.Column<string>(type: "text", nullable: false),
                    CostPerGram = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrintMaterials", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    HashedId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    SignUpTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.HashedId);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Key = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserHashedId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    EndTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Key);
                    table.ForeignKey(
                        name: "FK_Permissions_Users_UserHashedId",
                        column: x => x.UserHashedId,
                        principalTable: "Users",
                        principalColumn: "HashedId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrintLog",
                columns: table => new
                {
                    Key = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserHashedId = table.Column<string>(type: "text", nullable: true),
                    Time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    MaterialName = table.Column<string>(type: "text", nullable: true),
                    WeightGrams = table.Column<float>(type: "real", nullable: false),
                    Purpose = table.Column<string>(type: "text", nullable: false),
                    BillTo = table.Column<string>(type: "text", nullable: true),
                    Cost = table.Column<float>(type: "real", nullable: false),
                    Owed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrintLog", x => x.Key);
                    table.ForeignKey(
                        name: "FK_PrintLog_PrintMaterials_MaterialName",
                        column: x => x.MaterialName,
                        principalTable: "PrintMaterials",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrintLog_Users_UserHashedId",
                        column: x => x.UserHashedId,
                        principalTable: "Users",
                        principalColumn: "HashedId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Key = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserHashedId = table.Column<string>(type: "text", nullable: true),
                    College = table.Column<string>(type: "text", nullable: true),
                    Year = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Key);
                    table.ForeignKey(
                        name: "FK_Students_Users_UserHashedId",
                        column: x => x.UserHashedId,
                        principalTable: "Users",
                        principalColumn: "HashedId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VisitLogs",
                columns: table => new
                {
                    Key = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserHashedId = table.Column<string>(type: "text", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: false),
                    Time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitLogs", x => x.Key);
                    table.ForeignKey(
                        name: "FK_VisitLogs_Users_UserHashedId",
                        column: x => x.UserHashedId,
                        principalTable: "Users",
                        principalColumn: "HashedId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_UserHashedId",
                table: "Permissions",
                column: "UserHashedId");

            migrationBuilder.CreateIndex(
                name: "IX_PrintLog_MaterialName",
                table: "PrintLog",
                column: "MaterialName");

            migrationBuilder.CreateIndex(
                name: "IX_PrintLog_UserHashedId",
                table: "PrintLog",
                column: "UserHashedId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_UserHashedId",
                table: "Students",
                column: "UserHashedId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitLogs_UserHashedId",
                table: "VisitLogs",
                column: "UserHashedId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "PrintLog");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "VisitLogs");

            migrationBuilder.DropTable(
                name: "PrintMaterials");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
