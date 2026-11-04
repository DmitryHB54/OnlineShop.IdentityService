using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineShop.IdentityService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SLetter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpireAt",
                table: "refresh_tokens",
                newName: "ExpiresAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpiresAt",
                table: "refresh_tokens",
                newName: "ExpireAt");
        }
    }
}
