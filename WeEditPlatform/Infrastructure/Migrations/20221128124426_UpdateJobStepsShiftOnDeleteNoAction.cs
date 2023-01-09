using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class UpdateJobStepsShiftOnDeleteNoAction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobSteps_Shifts_ShiftId",
                table: "JobSteps");

            migrationBuilder.AddForeignKey(
                name: "FK_JobSteps_Shifts_ShiftId",
                table: "JobSteps",
                column: "ShiftId",
                principalTable: "Shifts",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobSteps_Shifts_ShiftId",
                table: "JobSteps");

            migrationBuilder.AddForeignKey(
                name: "FK_JobSteps_Shifts_ShiftId",
                table: "JobSteps",
                column: "ShiftId",
                principalTable: "Shifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
