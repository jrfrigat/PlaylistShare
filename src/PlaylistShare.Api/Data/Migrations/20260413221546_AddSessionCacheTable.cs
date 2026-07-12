using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlaylistShare.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSessionCacheTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE TABLE [dbo].[SessionCache] (
                    [Id] NVARCHAR(449) NOT NULL,
                    [Value] VARBINARY(MAX) NOT NULL,
                    [ExpiresAtTime] DATETIMEOFFSET NOT NULL,
                    [SlidingExpirationInSeconds] BIGINT NULL,
                    [AbsoluteExpiration] DATETIMEOFFSET NULL,
                    CONSTRAINT [pk_SessionCache] PRIMARY KEY ([Id])
                );
                CREATE NONCLUSTERED INDEX [Index_ExpiresAtTime] ON [dbo].[SessionCache] ([ExpiresAtTime]);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TABLE [dbo].[SessionCache]");
        }
    }
}
