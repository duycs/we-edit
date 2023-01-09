using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class UpdateStaffShiftHasIdPKAuto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE StaffShifts DROP PRIMARY KEY;");
            migrationBuilder.Sql(@"ALTER TABLE StaffShifts modify COLUMN Id INT UNSIGNED NOT NULL AUTO_INCREMENT, ADD INDEX (Id);");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
