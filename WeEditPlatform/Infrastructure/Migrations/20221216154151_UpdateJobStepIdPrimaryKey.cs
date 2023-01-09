using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class UpdateJobStepIdPrimaryKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE JobSteps DROP PRIMARY KEY;");
            migrationBuilder.Sql(@"ALTER TABLE JobSteps modify COLUMN Id INT UNSIGNED NOT NULL AUTO_INCREMENT, ADD INDEX (Id);");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
