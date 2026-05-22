using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Admission.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MakeDocumentFileNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_educational_documents_files_file_id",
                table: "educational_documents");

            migrationBuilder.DropForeignKey(
                name: "FK_passports_files_file_id",
                table: "passports");

            migrationBuilder.AlterColumn<Guid>(
                name: "file_id",
                table: "passports",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "file_id",
                table: "educational_documents",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_educational_documents_files_file_id",
                table: "educational_documents",
                column: "file_id",
                principalTable: "files",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_passports_files_file_id",
                table: "passports",
                column: "file_id",
                principalTable: "files",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_educational_documents_files_file_id",
                table: "educational_documents");

            migrationBuilder.DropForeignKey(
                name: "FK_passports_files_file_id",
                table: "passports");

            migrationBuilder.AlterColumn<Guid>(
                name: "file_id",
                table: "passports",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "file_id",
                table: "educational_documents",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_educational_documents_files_file_id",
                table: "educational_documents",
                column: "file_id",
                principalTable: "files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_passports_files_file_id",
                table: "passports",
                column: "file_id",
                principalTable: "files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
