using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlaylistShare.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class DropUserSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrackAdditionLogs_UserSessions_SessionId",
                table: "TrackAdditionLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_TrackRemovalLogs_UserSessions_SessionId",
                table: "TrackRemovalLogs");

            migrationBuilder.DropTable(
                name: "UserSessions");

            migrationBuilder.DropIndex(
                name: "IX_TrackRemovalLogs_SessionId",
                table: "TrackRemovalLogs");

            migrationBuilder.DropIndex(
                name: "IX_TrackAdditionLogs_SessionId",
                table: "TrackAdditionLogs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserSessions",
                columns: table => new
                {
                    SessionId = table.Column<string>(type: "nvarchar(449)", maxLength: 449, nullable: false),
                    AssociatedUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ClientIpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstSeenUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastSeenUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessions", x => x.SessionId);
                    table.ForeignKey(
                        name: "FK_UserSessions_AspNetUsers_AssociatedUserId",
                        column: x => x.AssociatedUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrackRemovalLogs_SessionId",
                table: "TrackRemovalLogs",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackAdditionLogs_SessionId",
                table: "TrackAdditionLogs",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_AssociatedUserId",
                table: "UserSessions",
                column: "AssociatedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrackAdditionLogs_UserSessions_SessionId",
                table: "TrackAdditionLogs",
                column: "SessionId",
                principalTable: "UserSessions",
                principalColumn: "SessionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TrackRemovalLogs_UserSessions_SessionId",
                table: "TrackRemovalLogs",
                column: "SessionId",
                principalTable: "UserSessions",
                principalColumn: "SessionId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
