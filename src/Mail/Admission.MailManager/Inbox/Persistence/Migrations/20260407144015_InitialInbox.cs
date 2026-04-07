using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Admission.MailManager.Inbox.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialInbox : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "inbox_messages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    contract_message_id = table.Column<Guid>(type: "uuid", nullable: false),
                    transport_message_id = table.Column<Guid>(type: "uuid", nullable: true),
                    correlation_id = table.Column<Guid>(type: "uuid", nullable: true),
                    conversation_id = table.Column<Guid>(type: "uuid", nullable: true),
                    message_type = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    payload_json = table.Column<string>(type: "jsonb", nullable: false),
                    headers_json = table.Column<string>(type: "jsonb", nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    received_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_attempt_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    locked_until_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    processed_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    attempt_count = table.Column<int>(type: "integer", nullable: false),
                    last_error = table.Column<string>(type: "text", nullable: true),
                    source_address = table.Column<string>(type: "text", nullable: true),
                    destination_address = table.Column<string>(type: "text", nullable: true),
                    sent_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inbox_messages", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_inbox_messages_contract_message_id",
                table: "inbox_messages",
                column: "contract_message_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_inbox_messages_status_received_at_utc",
                table: "inbox_messages",
                columns: new[] { "status", "received_at_utc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "inbox_messages");
        }
    }
}
