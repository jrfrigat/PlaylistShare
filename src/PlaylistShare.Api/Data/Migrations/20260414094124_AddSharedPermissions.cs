using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlaylistShare.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSharedPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlayPermission",
                table: "SharedPlaylists",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlayPermission",
                table: "SharedPlaylists");
        }
    }
}
