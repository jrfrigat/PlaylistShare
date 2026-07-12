using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlaylistShare.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserFavorites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "YandexPlaylistUuid",
                table: "SharedPlaylists",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "FavoritePlaylists",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SharedPlaylistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AddedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoritePlaylists", x => new { x.UserId, x.SharedPlaylistId });
                    table.ForeignKey(
                        name: "FK_FavoritePlaylists_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FavoritePlaylists_SharedPlaylists_SharedPlaylistId",
                        column: x => x.SharedPlaylistId,
                        principalTable: "SharedPlaylists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FavoritePlaylists_SharedPlaylistId",
                table: "FavoritePlaylists",
                column: "SharedPlaylistId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavoritePlaylists");

            migrationBuilder.DropColumn(
                name: "YandexPlaylistUuid",
                table: "SharedPlaylists");
        }
    }
}
