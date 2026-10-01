using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EducationSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SomeUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Schools_SchoolId1",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SchoolId1",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SchoolId1",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SchoolId1",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SchoolId1",
                table: "AspNetUsers",
                column: "SchoolId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Schools_SchoolId1",
                table: "AspNetUsers",
                column: "SchoolId1",
                principalTable: "Schools",
                principalColumn: "Id");
        }
    }
}
