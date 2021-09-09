using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cashbox.Migrations
{
    public partial class _5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateAndTime",
                table: "Shifts",
                newName: "LastModified");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Shifts",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Shifts");

            migrationBuilder.RenameColumn(
                name: "LastModified",
                table: "Shifts",
                newName: "DateAndTime");
        }
    }
}
