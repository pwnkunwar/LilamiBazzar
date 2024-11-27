using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LilamiBazzar.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddingItemTrackingtbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Auctions_ProductId",
                table: "Auctions");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("79186627-a2f0-4093-8e8f-fe07606a4482"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("e951f3a3-e43c-4516-abee-f5b1e1d76198"));

            migrationBuilder.CreateTable(
                name: "ItemTracking",
                columns: table => new
                {
                    ItemTrackingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BuyerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SellerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ShippingProvider = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrackingNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstimatedDeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeliverdAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemTracking", x => x.ItemTrackingId);
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("8c70f334-1a8d-4c1c-a3d6-2be077061188"), "Regular user role", "USER" },
                    { new Guid("bc4d28d9-a5df-416b-b6b4-894b7f61162e"), "Administrator role", "ADMIN" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Auctions_ProductId",
                table: "Auctions",
                column: "ProductId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemTracking");

            migrationBuilder.DropIndex(
                name: "IX_Auctions_ProductId",
                table: "Auctions");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("8c70f334-1a8d-4c1c-a3d6-2be077061188"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("bc4d28d9-a5df-416b-b6b4-894b7f61162e"));

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("79186627-a2f0-4093-8e8f-fe07606a4482"), "Regular user role", "USER" },
                    { new Guid("e951f3a3-e43c-4516-abee-f5b1e1d76198"), "Administrator role", "ADMIN" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Auctions_ProductId",
                table: "Auctions",
                column: "ProductId");
        }
    }
}
