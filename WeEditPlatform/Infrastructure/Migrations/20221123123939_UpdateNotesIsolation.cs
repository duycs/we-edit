using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class UpdateNotesIsolation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobSteps_Notes_NoteId",
                table: "JobSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_JobSteps_Shifts_ShiftId",
                table: "JobSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_JobSteps_Steps_StepId",
                table: "JobSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_Notes_Staffs_NoterId",
                table: "Notes");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffProductLevels_ProductLevels_ProductLevelId",
                table: "StaffProductLevels");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffProductLevels_Staffs_StaffId",
                table: "StaffProductLevels");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffRoles_Roles_RoleId",
                table: "StaffRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffShifts_Shifts_ShiftId",
                table: "StaffShifts");

            migrationBuilder.DropIndex(
                name: "IX_Notes_NoterId",
                table: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_JobSteps_NoteId",
                table: "JobSteps");

            migrationBuilder.DropColumn(
                name: "NoteId",
                table: "JobSteps");

            migrationBuilder.AddColumn<int>(
                name: "ObjectId",
                table: "Notes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ObjectName",
                table: "Notes",
                type: "longtext",
                nullable: false);

            migrationBuilder.AlterColumn<int>(
                name: "ShiftId",
                table: "JobSteps",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_JobSteps_Shifts_ShiftId",
                table: "JobSteps",
                column: "ShiftId",
                principalTable: "Shifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobSteps_Steps_StepId",
                table: "JobSteps",
                column: "StepId",
                principalTable: "Steps",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StaffProductLevels_ProductLevels_ProductLevelId",
                table: "StaffProductLevels",
                column: "ProductLevelId",
                principalTable: "ProductLevels",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StaffProductLevels_Staffs_StaffId",
                table: "StaffProductLevels",
                column: "StaffId",
                principalTable: "Staffs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StaffRoles_Roles_RoleId",
                table: "StaffRoles",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StaffShifts_Shifts_ShiftId",
                table: "StaffShifts",
                column: "ShiftId",
                principalTable: "Shifts",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobSteps_Shifts_ShiftId",
                table: "JobSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_JobSteps_Steps_StepId",
                table: "JobSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffProductLevels_ProductLevels_ProductLevelId",
                table: "StaffProductLevels");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffProductLevels_Staffs_StaffId",
                table: "StaffProductLevels");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffRoles_Roles_RoleId",
                table: "StaffRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffShifts_Shifts_ShiftId",
                table: "StaffShifts");

            migrationBuilder.DropColumn(
                name: "ObjectId",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "ObjectName",
                table: "Notes");

            migrationBuilder.AlterColumn<int>(
                name: "ShiftId",
                table: "JobSteps",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "NoteId",
                table: "JobSteps",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Notes_NoterId",
                table: "Notes",
                column: "NoterId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSteps_NoteId",
                table: "JobSteps",
                column: "NoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobSteps_Notes_NoteId",
                table: "JobSteps",
                column: "NoteId",
                principalTable: "Notes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobSteps_Shifts_ShiftId",
                table: "JobSteps",
                column: "ShiftId",
                principalTable: "Shifts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JobSteps_Steps_StepId",
                table: "JobSteps",
                column: "StepId",
                principalTable: "Steps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_Staffs_NoterId",
                table: "Notes",
                column: "NoterId",
                principalTable: "Staffs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StaffProductLevels_ProductLevels_ProductLevelId",
                table: "StaffProductLevels",
                column: "ProductLevelId",
                principalTable: "ProductLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StaffProductLevels_Staffs_StaffId",
                table: "StaffProductLevels",
                column: "StaffId",
                principalTable: "Staffs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StaffRoles_Roles_RoleId",
                table: "StaffRoles",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StaffShifts_Shifts_ShiftId",
                table: "StaffShifts",
                column: "ShiftId",
                principalTable: "Shifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
