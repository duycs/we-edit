using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class updateRefStaffRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StaffRoles_Staffs_RoleId",
                table: "StaffRoles");

            migrationBuilder.AddForeignKey(
                name: "FK_StaffRoles_Staffs_StaffId",
                table: "StaffRoles",
                column: "StaffId",
                principalTable: "Staffs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StaffRoles_Staffs_StaffId",
                table: "StaffRoles");

            migrationBuilder.AddForeignKey(
                name: "FK_StaffRoles_Staffs_RoleId",
                table: "StaffRoles",
                column: "RoleId",
                principalTable: "Staffs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
