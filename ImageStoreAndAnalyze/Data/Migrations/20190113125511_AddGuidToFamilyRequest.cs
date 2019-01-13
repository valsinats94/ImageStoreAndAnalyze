using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImageStoreAndAnalyze.Data.Migrations
{
    public partial class AddGuidToFamilyRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "FamilyRequests",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Guid",
                table: "FamilyRequests");
        }
    }
}
