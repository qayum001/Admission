using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Admission.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCitizenshipAndManagerEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "managers",
                type: "character varying(320)",
                maxLength: 320,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "citizenship",
                table: "applicants",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "email",
                table: "managers");

            migrationBuilder.DropColumn(
                name: "citizenship",
                table: "applicants");
        }
    }
}
