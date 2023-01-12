using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class UpdateNameFlows : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Operations_Flow_FlowId",
                table: "Operations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Flow",
                table: "Flow");

            migrationBuilder.RenameTable(
                name: "Flow",
                newName: "Flows");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Flows",
                table: "Flows",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Operations_Flows_FlowId",
                table: "Operations",
                column: "FlowId",
                principalTable: "Flows",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Operations_Flows_FlowId",
                table: "Operations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Flows",
                table: "Flows");

            migrationBuilder.RenameTable(
                name: "Flows",
                newName: "Flow");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Flow",
                table: "Flow",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Operations_Flow_FlowId",
                table: "Operations",
                column: "FlowId",
                principalTable: "Flow",
                principalColumn: "Id");
        }
    }
}
