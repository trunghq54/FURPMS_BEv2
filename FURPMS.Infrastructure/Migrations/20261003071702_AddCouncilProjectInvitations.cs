using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FURPMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCouncilProjectInvitations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "council_project_invitations",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    council_member_id = table.Column<Guid>(type: "uuid", nullable: false),
                    council_id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    invited_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    responded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    decline_reason = table.Column<string>(type: "text", nullable: true),
                    responded_on_behalf_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_council_project_invitations", x => x.id);
                    table.ForeignKey(
                        name: "fk_council_project_invitations_council_members_council_member_",
                        column: x => x.council_member_id,
                        principalTable: "council_members",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_council_project_invitations_council_id_project_id",
                table: "council_project_invitations",
                columns: new[] { "council_id", "project_id" });

            migrationBuilder.CreateIndex(
                name: "ix_council_project_invitations_council_member_id_project_id",
                table: "council_project_invitations",
                columns: new[] { "council_member_id", "project_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "council_project_invitations");
        }
    }
}
