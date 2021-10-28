using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

namespace Construct.Core.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class SqliteMakePrintLogUserOptional : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrintLog_Users_UserHashedId",
                table: "PrintLog");

            migrationBuilder.AlterColumn<string>(
                name: "UserHashedId",
                table: "PrintLog",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddForeignKey(
                name: "FK_PrintLog_Users_UserHashedId",
                table: "PrintLog",
                column: "UserHashedId",
                principalTable: "Users",
                principalColumn: "HashedId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrintLog_Users_UserHashedId",
                table: "PrintLog");

            migrationBuilder.AlterColumn<string>(
                name: "UserHashedId",
                table: "PrintLog",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PrintLog_Users_UserHashedId",
                table: "PrintLog",
                column: "UserHashedId",
                principalTable: "Users",
                principalColumn: "HashedId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
