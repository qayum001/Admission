using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Admission.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "applicants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    birth_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    gender = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    phone_number = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_applicants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "education_levels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_education_levels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "faculties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    create_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_faculties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "files",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    key = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_files", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "education_document_types",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    create_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    education_level_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_education_document_types", x => x.Id);
                    table.ForeignKey(
                        name: "FK_education_document_types_education_levels_education_level_id",
                        column: x => x.education_level_id,
                        principalTable: "education_levels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "education_programs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    create_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    language = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    education_form = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    faculty_id = table.Column<Guid>(type: "uuid", nullable: false),
                    education_level_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_education_programs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_education_programs_education_levels_education_level_id",
                        column: x => x.education_level_id,
                        principalTable: "education_levels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_education_programs_faculties_faculty_id",
                        column: x => x.faculty_id,
                        principalTable: "faculties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "managers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    faculty_id = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_managers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_managers_faculties_faculty_id",
                        column: x => x.faculty_id,
                        principalTable: "faculties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "passports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    serial_number = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    given_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    given_by = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    applicant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_passports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_passports_applicants_applicant_id",
                        column: x => x.applicant_id,
                        principalTable: "applicants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_passports_files_file_id",
                        column: x => x.file_id,
                        principalTable: "files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "education_document_type_next_levels",
                columns: table => new
                {
                    education_document_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    next_education_level_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_education_document_type_next_levels", x => new { x.education_document_type_id, x.next_education_level_id });
                    table.ForeignKey(
                        name: "FK_education_document_type_next_levels_education_document_type~",
                        column: x => x.education_document_type_id,
                        principalTable: "education_document_types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_education_document_type_next_levels_education_levels_next_e~",
                        column: x => x.next_education_level_id,
                        principalTable: "education_levels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "educational_documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    education_document_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    applicant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_educational_documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_educational_documents_applicants_applicant_id",
                        column: x => x.applicant_id,
                        principalTable: "applicants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_educational_documents_education_document_types_education_do~",
                        column: x => x.education_document_type_id,
                        principalTable: "education_document_types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_educational_documents_files_file_id",
                        column: x => x.file_id,
                        principalTable: "files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "admissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    applicant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    manager_id = table.Column<Guid>(type: "uuid", nullable: true),
                    ManagerId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_admissions_applicants_applicant_id",
                        column: x => x.applicant_id,
                        principalTable: "applicants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_admissions_managers_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "managers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_admissions_managers_manager_id",
                        column: x => x.manager_id,
                        principalTable: "managers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "admission_programs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    program_id = table.Column<Guid>(type: "uuid", nullable: false),
                    admission_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admission_programs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_admission_programs_admissions_admission_id",
                        column: x => x.admission_id,
                        principalTable: "admissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_admission_programs_education_programs_program_id",
                        column: x => x.program_id,
                        principalTable: "education_programs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_admission_programs_admission_id_priority",
                table: "admission_programs",
                columns: new[] { "admission_id", "priority" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_admission_programs_program_id",
                table: "admission_programs",
                column: "program_id");

            migrationBuilder.CreateIndex(
                name: "IX_admissions_applicant_id",
                table: "admissions",
                column: "applicant_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_admissions_manager_id",
                table: "admissions",
                column: "manager_id");

            migrationBuilder.CreateIndex(
                name: "IX_admissions_ManagerId",
                table: "admissions",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_education_document_type_next_levels_next_education_level_id",
                table: "education_document_type_next_levels",
                column: "next_education_level_id");

            migrationBuilder.CreateIndex(
                name: "IX_education_document_types_education_level_id",
                table: "education_document_types",
                column: "education_level_id");

            migrationBuilder.CreateIndex(
                name: "IX_education_programs_code",
                table: "education_programs",
                column: "code");

            migrationBuilder.CreateIndex(
                name: "IX_education_programs_education_level_id",
                table: "education_programs",
                column: "education_level_id");

            migrationBuilder.CreateIndex(
                name: "IX_education_programs_faculty_id",
                table: "education_programs",
                column: "faculty_id");

            migrationBuilder.CreateIndex(
                name: "IX_educational_documents_applicant_id",
                table: "educational_documents",
                column: "applicant_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_educational_documents_education_document_type_id",
                table: "educational_documents",
                column: "education_document_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_educational_documents_file_id",
                table: "educational_documents",
                column: "file_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_files_key",
                table: "files",
                column: "key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_managers_faculty_id",
                table: "managers",
                column: "faculty_id");

            migrationBuilder.CreateIndex(
                name: "IX_passports_applicant_id",
                table: "passports",
                column: "applicant_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_passports_file_id",
                table: "passports",
                column: "file_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "admission_programs");

            migrationBuilder.DropTable(
                name: "education_document_type_next_levels");

            migrationBuilder.DropTable(
                name: "educational_documents");

            migrationBuilder.DropTable(
                name: "passports");

            migrationBuilder.DropTable(
                name: "admissions");

            migrationBuilder.DropTable(
                name: "education_programs");

            migrationBuilder.DropTable(
                name: "education_document_types");

            migrationBuilder.DropTable(
                name: "files");

            migrationBuilder.DropTable(
                name: "applicants");

            migrationBuilder.DropTable(
                name: "managers");

            migrationBuilder.DropTable(
                name: "education_levels");

            migrationBuilder.DropTable(
                name: "faculties");
        }
    }
}
