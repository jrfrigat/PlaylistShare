using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlaylistShare.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddYandexAuthSessions_Header : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HeaderCsfrToken",
                table: "YandexAuthSessions",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeaderProcessId",
                table: "YandexAuthSessions",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HeaderCsfrToken",
                table: "YandexAuthSessions");

            migrationBuilder.DropColumn(
                name: "HeaderProcessId",
                table: "YandexAuthSessions");
        }
    }
}
