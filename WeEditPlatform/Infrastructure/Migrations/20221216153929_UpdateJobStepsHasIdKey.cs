using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class UpdateJobStepsHasIdKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_JobSteps",
                table: "JobSteps");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "JobSteps",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "Id",
                table: "JobSteps",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_JobSteps_JobId",
                table: "JobSteps",
                column: "JobId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "Id",
                table: "JobSteps");

            migrationBuilder.DropIndex(
                name: "IX_JobSteps_JobId",
                table: "JobSteps");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "JobSteps");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobSteps",
                table: "JobSteps",
                columns: new[] { "JobId", "StepId" });
        }
    }
}
