using Microsoft.EntityFrameworkCore.Migrations;

namespace Cashbox.Migrations
{
    public partial class _5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShiftUser_Shift_ShiftsId",
                table: "ShiftUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Shift",
                table: "Shift");

            migrationBuilder.RenameTable(
                name: "Shift",
                newName: "Shifts");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Shifts",
                table: "Shifts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftUser_Shifts_ShiftsId",
                table: "ShiftUser",
                column: "ShiftsId",
                principalTable: "Shifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShiftUser_Shifts_ShiftsId",
                table: "ShiftUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Shifts",
                table: "Shifts");

            migrationBuilder.RenameTable(
                name: "Shifts",
                newName: "Shift");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Shift",
                table: "Shift",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftUser_Shift_ShiftsId",
                table: "ShiftUser",
                column: "ShiftsId",
                principalTable: "Shift",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
