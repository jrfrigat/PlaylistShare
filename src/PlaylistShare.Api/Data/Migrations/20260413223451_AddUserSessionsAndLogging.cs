using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlaylistShare.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserSessionsAndLogging : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "AddedByUserId",
                table: "TrackAdditionLogs",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<string>(
                name: "SessionId",
                table: "TrackAdditionLogs",
                type: "nvarchar(449)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "UserSessions",
                columns: table => new
                {
                    SessionId = table.Column<string>(type: "nvarchar(449)", maxLength: 449, nullable: false),
                    ClientIpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstSeenUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastSeenUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AssociatedUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "TrackRemovalLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SharedPlaylistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrackId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RemovedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RemovedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SessionId = table.Column<string>(type: "nvarchar(449)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackRemovalLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrackRemovalLogs_AspNetUsers_RemovedByUserId",
                        column: x => x.RemovedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrackRemovalLogs_SharedPlaylists_SharedPlaylistId",
                        column: x => x.SharedPlaylistId,
                        principalTable: "SharedPlaylists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrackRemovalLogs_UserSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "UserSessions",
                        principalColumn: "SessionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrackAdditionLogs_SessionId",
                table: "TrackAdditionLogs",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackRemovalLogs_RemovedByUserId",
                table: "TrackRemovalLogs",
                column: "RemovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackRemovalLogs_SessionId",
                table: "TrackRemovalLogs",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackRemovalLogs_SharedPlaylistId_TrackId",
                table: "TrackRemovalLogs",
                columns: new[] { "SharedPlaylistId", "TrackId" });

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrackAdditionLogs_UserSessions_SessionId",
                table: "TrackAdditionLogs");

            migrationBuilder.DropTable(
                name: "TrackRemovalLogs");

            migrationBuilder.DropTable(
                name: "UserSessions");

            migrationBuilder.DropIndex(
                name: "IX_TrackAdditionLogs_SessionId",
                table: "TrackAdditionLogs");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "TrackAdditionLogs");

            migrationBuilder.AlterColumn<Guid>(
                name: "AddedByUserId",
                table: "TrackAdditionLogs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
