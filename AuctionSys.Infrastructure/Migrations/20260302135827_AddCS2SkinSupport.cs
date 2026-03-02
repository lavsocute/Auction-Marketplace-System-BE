using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuctionSys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCS2SkinSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssetId",
                table: "Items",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Bots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SteamId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TradeUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', now())"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SkinMetadata",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Exterior = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FloatValue = table.Column<decimal>(type: "numeric(10,8)", precision: 10, scale: 8, nullable: true),
                    PatternIndex = table.Column<int>(type: "integer", nullable: true),
                    IsStatTrak = table.Column<bool>(type: "boolean", nullable: false),
                    NameTag = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    StickersJson = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkinMetadata", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SkinMetadata_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BotInventories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BotId = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    TradeLockedUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', now())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotInventories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BotInventories_Bots_BotId",
                        column: x => x.BotId,
                        principalTable: "Bots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BotInventories_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BotInventories_BotId",
                table: "BotInventories",
                column: "BotId");

            migrationBuilder.CreateIndex(
                name: "IX_BotInventories_ItemId",
                table: "BotInventories",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_BotInventories_TradeLockedUntil",
                table: "BotInventories",
                column: "TradeLockedUntil");

            migrationBuilder.CreateIndex(
                name: "IX_Bots_SteamId",
                table: "Bots",
                column: "SteamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SkinMetadata_ItemId",
                table: "SkinMetadata",
                column: "ItemId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BotInventories");

            migrationBuilder.DropTable(
                name: "SkinMetadata");

            migrationBuilder.DropTable(
                name: "Bots");

            migrationBuilder.DropColumn(
                name: "AssetId",
                table: "Items");
        }
    }
}
