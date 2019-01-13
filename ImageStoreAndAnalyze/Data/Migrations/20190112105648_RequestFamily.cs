using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImageStoreAndAnalyze.Data.Migrations
{
    public partial class RequestFamily : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FamilyRequests",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SendDate = table.Column<DateTime>(nullable: false),
                    RequestedFamilyID = table.Column<int>(nullable: false),
                    RequestByUserId = table.Column<string>(nullable: false),
                    IsProcessed = table.Column<bool>(nullable: false),
                    ProcessedDate = table.Column<DateTime>(nullable: false),
                    ProcessResult = table.Column<int>(nullable: false),
                    ProcessedByUserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FamilyRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FamilyRequests_AspNetUsers_ProcessedByUserId",
                        column: x => x.ProcessedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FamilyRequests_AspNetUsers_RequestByUserId",
                        column: x => x.RequestByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FamilyRequests_Families_RequestedFamilyID",
                        column: x => x.RequestedFamilyID,
                        principalTable: "Families",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FamilyRequests_ProcessedByUserId",
                table: "FamilyRequests",
                column: "ProcessedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FamilyRequests_RequestByUserId",
                table: "FamilyRequests",
                column: "RequestByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FamilyRequests_RequestedFamilyID",
                table: "FamilyRequests",
                column: "RequestedFamilyID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FamilyRequests");
        }
    }
}
