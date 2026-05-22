using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Admission.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DropUniquePriorityConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_admission_programs_admission_id_priority",
                table: "admission_programs");

            migrationBuilder.CreateIndex(
                name: "IX_admission_programs_admission_id_priority",
                table: "admission_programs",
                columns: new[] { "admission_id", "priority" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_admission_programs_admission_id_priority",
                table: "admission_programs");

            migrationBuilder.CreateIndex(
                name: "IX_admission_programs_admission_id_priority",
                table: "admission_programs",
                columns: new[] { "admission_id", "priority" },
                unique: true);
        }
    }
}
