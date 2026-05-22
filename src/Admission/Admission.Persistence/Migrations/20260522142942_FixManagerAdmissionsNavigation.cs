using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Admission.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixManagerAdmissionsNavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_admissions_managers_ManagerId",
                table: "admissions");

            migrationBuilder.DropIndex(
                name: "IX_admissions_ManagerId",
                table: "admissions");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "admissions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ManagerId",
                table: "admissions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_admissions_ManagerId",
                table: "admissions",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_admissions_managers_ManagerId",
                table: "admissions",
                column: "ManagerId",
                principalTable: "managers",
                principalColumn: "Id");
        }
    }
}
