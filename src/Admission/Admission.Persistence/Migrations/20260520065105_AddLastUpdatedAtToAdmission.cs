using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Admission.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLastUpdatedAtToAdmission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "last_updated_at",
                table: "admissions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.Sql("UPDATE admissions SET last_updated_at = NOW() WHERE last_updated_at = '-infinity' OR last_updated_at < '2000-01-01'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "last_updated_at",
                table: "admissions");
        }
    }
}
