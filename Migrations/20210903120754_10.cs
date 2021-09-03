using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cashbox.Migrations
{
    public partial class _10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "Shifts");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateAndTime",
                table: "Shifts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateAndTime",
                table: "Shifts");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Shifts",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
