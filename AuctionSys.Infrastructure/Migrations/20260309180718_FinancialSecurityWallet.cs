using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuctionSys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FinancialSecurityWallet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Signature",
                table: "Wallets",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Signature",
                table: "Wallets");
        }
    }
}
