using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class UpdateStaffShiftKeyId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StaffShifts",
                table: "StaffShifts");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "StaffShifts",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "Id",
                table: "StaffShifts",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_StaffShifts_StaffId",
                table: "StaffShifts",
                column: "StaffId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "Id",
                table: "StaffShifts");

            migrationBuilder.DropIndex(
                name: "IX_StaffShifts_StaffId",
                table: "StaffShifts");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "StaffShifts");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StaffShifts",
                table: "StaffShifts",
                columns: new[] { "StaffId", "ShiftId" });
        }
    }
}
