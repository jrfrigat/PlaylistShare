using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlaylistShare.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class OptimizeBackendSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataProtectionKeys");

            migrationBuilder.DropColumn(
                name: "CsfrToken",
                table: "YandexAuthSessions");

            migrationBuilder.DropColumn(
                name: "HeaderCsfrToken",
                table: "YandexAuthSessions");

            migrationBuilder.DropColumn(
                name: "HeaderProcessId",
                table: "YandexAuthSessions");

            migrationBuilder.DropColumn(
                name: "SerializedCookies",
                table: "YandexAuthSessions");

            migrationBuilder.DropColumn(
                name: "TrackId",
                table: "YandexAuthSessions");

            migrationBuilder.AlterColumn<string>(
                name: "RefreshToken",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_RefreshToken",
                table: "AspNetUsers",
                column: "RefreshToken");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_RefreshToken",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "CsfrToken",
                table: "YandexAuthSessions",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

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

            migrationBuilder.AddColumn<string>(
                name: "SerializedCookies",
                table: "YandexAuthSessions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TrackId",
                table: "YandexAuthSessions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RefreshToken",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "DataProtectionKeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FriendlyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Xml = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataProtectionKeys", x => x.Id);
                });
        }
    }
}
