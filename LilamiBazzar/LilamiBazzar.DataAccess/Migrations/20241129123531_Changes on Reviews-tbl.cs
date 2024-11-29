using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LilamiBazzar.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ChangesonReviewstbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("8c70f334-1a8d-4c1c-a3d6-2be077061188"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("bc4d28d9-a5df-416b-b6b4-894b7f61162e"));

            migrationBuilder.DropColumn(
                name: "NegativeRating",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "PositiveRating",
                table: "Reviews");

            migrationBuilder.AddColumn<string>(
                name: "feedback",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("6ed3838f-47aa-48d4-be57-713d96ff9d3f"), "Regular user role", "USER" },
                    { new Guid("b9a96a7e-981e-4dbc-9a8d-aee8ce3d949b"), "Administrator role", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("6ed3838f-47aa-48d4-be57-713d96ff9d3f"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("b9a96a7e-981e-4dbc-9a8d-aee8ce3d949b"));

            migrationBuilder.DropColumn(
                name: "feedback",
                table: "Reviews");

            migrationBuilder.AddColumn<int>(
                name: "NegativeRating",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PositiveRating",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("8c70f334-1a8d-4c1c-a3d6-2be077061188"), "Regular user role", "USER" },
                    { new Guid("bc4d28d9-a5df-416b-b6b4-894b7f61162e"), "Administrator role", "ADMIN" }
                });
        }
    }
}
