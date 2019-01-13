using Microsoft.EntityFrameworkCore.Migrations;

namespace ImageStoreAndAnalyze.Data.Migrations
{
    public partial class IsDeletedRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "FamilyRequests",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "FamilyRequests");
        }
    }
}
