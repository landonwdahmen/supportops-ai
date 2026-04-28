using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupportOpsAI.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPhase3TriageWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CorrelationId",
                table: "triage_jobs",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastError",
                table: "triage_jobs",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewNotes",
                table: "ticket_triage_results",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ReviewedAt",
                table: "ticket_triage_results",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReviewedByUserId",
                table: "ticket_triage_results",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SuggestedSteps",
                table: "ticket_triage_results",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_triage_jobs_CorrelationId",
                table: "triage_jobs",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_ticket_triage_results_ReviewedByUserId",
                table: "ticket_triage_results",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ticket_triage_results_ReviewStatus",
                table: "ticket_triage_results",
                column: "ReviewStatus");

            migrationBuilder.AddForeignKey(
                name: "FK_ticket_triage_results_users_ReviewedByUserId",
                table: "ticket_triage_results",
                column: "ReviewedByUserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ticket_triage_results_users_ReviewedByUserId",
                table: "ticket_triage_results");

            migrationBuilder.DropIndex(
                name: "IX_triage_jobs_CorrelationId",
                table: "triage_jobs");

            migrationBuilder.DropIndex(
                name: "IX_ticket_triage_results_ReviewedByUserId",
                table: "ticket_triage_results");

            migrationBuilder.DropIndex(
                name: "IX_ticket_triage_results_ReviewStatus",
                table: "ticket_triage_results");

            migrationBuilder.DropColumn(
                name: "CorrelationId",
                table: "triage_jobs");

            migrationBuilder.DropColumn(
                name: "LastError",
                table: "triage_jobs");

            migrationBuilder.DropColumn(
                name: "ReviewNotes",
                table: "ticket_triage_results");

            migrationBuilder.DropColumn(
                name: "ReviewedAt",
                table: "ticket_triage_results");

            migrationBuilder.DropColumn(
                name: "ReviewedByUserId",
                table: "ticket_triage_results");

            migrationBuilder.DropColumn(
                name: "SuggestedSteps",
                table: "ticket_triage_results");
        }
    }
}
