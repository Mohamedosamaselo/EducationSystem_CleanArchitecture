using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EducationSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditColumnsToTeacherGrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "UserSubjects",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "UserSubjects",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "UserSubjects",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "UserSubjects",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "UserSubjects",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "TeacherGrades",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "TeacherGrades",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "TeacherGrades",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "TeacherGrades",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "UserSubjects");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "UserSubjects");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserSubjects");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "UserSubjects");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "UserSubjects");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "TeacherGrades");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "TeacherGrades");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "TeacherGrades");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "TeacherGrades");
        }
    }
}
