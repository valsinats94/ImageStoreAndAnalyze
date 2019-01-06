using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ImageStoreAndAnalyze.Data.Migrations
{
    public partial class AddFamilyAdmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FamilyAdministratorId",
                table: "Families",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Families_FamilyAdministratorId",
                table: "Families",
                column: "FamilyAdministratorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Families_AspNetUsers_FamilyAdministratorId",
                table: "Families",
                column: "FamilyAdministratorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Families_AspNetUsers_FamilyAdministratorId",
                table: "Families");

            migrationBuilder.DropIndex(
                name: "IX_Families_FamilyAdministratorId",
                table: "Families");

            migrationBuilder.DropColumn(
                name: "FamilyAdministratorId",
                table: "Families");
        }
    }
}
