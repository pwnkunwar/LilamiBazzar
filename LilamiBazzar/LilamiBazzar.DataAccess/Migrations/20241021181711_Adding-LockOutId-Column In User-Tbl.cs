using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LilamiBazzar.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddingLockOutIdColumnInUserTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("171d7b89-e6de-4f75-b24f-a54b4a121a21"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("cc68dacf-4d1e-4ad3-acd4-5fcf72a477ea"));

            migrationBuilder.DropColumn(
                name: "LockoutEnd",
                table: "Users");

            migrationBuilder.AddColumn<Guid>(
                name: "LockoutId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("7c55f045-7a18-4416-8120-81ed646cf627"), "Regular user role", "USER" },
                    { new Guid("8a066e9c-5fe5-418a-a933-eda7b4377c9b"), "Administrator role", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("7c55f045-7a18-4416-8120-81ed646cf627"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("8a066e9c-5fe5-418a-a933-eda7b4377c9b"));

            migrationBuilder.DropColumn(
                name: "LockoutId",
                table: "Users");

            migrationBuilder.AddColumn<DateTime>(
                name: "LockoutEnd",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("171d7b89-e6de-4f75-b24f-a54b4a121a21"), "Regular user role", "USER" },
                    { new Guid("cc68dacf-4d1e-4ad3-acd4-5fcf72a477ea"), "Administrator role", "ADMIN" }
                });
        }
    }
}
