using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EducationSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddingSchoolAdminId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SchoolAdminId",
                table: "Schools",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetRoles",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.CreateIndex(
                name: "IX_Schools_SchoolAdminId",
                table: "Schools",
                column: "SchoolAdminId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Schools_AspNetUsers_SchoolAdminId",
                table: "Schools",
                column: "SchoolAdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schools_AspNetUsers_SchoolAdminId",
                table: "Schools");

            migrationBuilder.DropIndex(
                name: "IX_Schools_SchoolAdminId",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "SchoolAdminId",
                table: "Schools");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetRoles",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);
        }
    }
}
