using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EducationSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class deleteColumnDescriptionFromTableAspRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "AspNetRoles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "AspNetRoles",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");
        }
    }
}
