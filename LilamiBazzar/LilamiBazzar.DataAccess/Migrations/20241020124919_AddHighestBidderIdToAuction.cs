using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LilamiBazzar.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddHighestBidderIdToAuction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("01549b0a-eb55-46c9-b094-1c2e68d43ee4"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("92a9a4e8-1027-4c21-a708-c98487089066"));

            migrationBuilder.AddColumn<Guid>(
                name: "HighestBidderId",
                table: "Auctions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("195e1adf-d35f-4a96-83aa-3d96df97aa95"), "Administrator role", "ADMIN" },
                    { new Guid("92c73f3e-16f1-4d6b-8ec5-ab423daef368"), "Regular user role", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("195e1adf-d35f-4a96-83aa-3d96df97aa95"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("92c73f3e-16f1-4d6b-8ec5-ab423daef368"));

            migrationBuilder.DropColumn(
                name: "HighestBidderId",
                table: "Auctions");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("01549b0a-eb55-46c9-b094-1c2e68d43ee4"), "Regular user role", "USER" },
                    { new Guid("92a9a4e8-1027-4c21-a708-c98487089066"), "Administrator role", "ADMIN" }
                });
        }
    }
}
