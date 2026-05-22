using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Admission.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserHierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "external_id",
                table: "managers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "role",
                table: "managers",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "Manager");

            migrationBuilder.AddColumn<Guid>(
                name: "external_id",
                table: "applicants",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "role",
                table: "applicants",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "Applicant");

            migrationBuilder.Sql("""
                UPDATE managers
                SET external_id = "Id"
                WHERE external_id = '00000000-0000-0000-0000-000000000000';
                """);

            migrationBuilder.Sql("""
                UPDATE applicants
                SET external_id = "Id"
                WHERE external_id = '00000000-0000-0000-0000-000000000000';
                """);

            migrationBuilder.AlterColumn<Guid>(
                name: "external_id",
                table: "managers",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "role",
                table: "managers",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64,
                oldDefaultValue: "Manager");

            migrationBuilder.AlterColumn<Guid>(
                name: "external_id",
                table: "applicants",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "role",
                table: "applicants",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64,
                oldDefaultValue: "Applicant");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "external_id",
                table: "managers");

            migrationBuilder.DropColumn(
                name: "role",
                table: "managers");

            migrationBuilder.DropColumn(
                name: "external_id",
                table: "applicants");

            migrationBuilder.DropColumn(
                name: "role",
                table: "applicants");
        }
    }
}
