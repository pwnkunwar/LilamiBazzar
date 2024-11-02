using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LilamiBazzar.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdatingProducttbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("7c55f045-7a18-4416-8120-81ed646cf627"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("8a066e9c-5fe5-418a-a933-eda7b4377c9b"));

            migrationBuilder.AddColumn<string>(
                name: "ProductRoles",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("65f7e4bf-0e75-4044-b72e-4bd9ae8c1841"), "Administrator role", "ADMIN" },
                    { new Guid("92861a16-367c-46eb-9db8-d20ecc93e38d"), "Regular user role", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("65f7e4bf-0e75-4044-b72e-4bd9ae8c1841"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("92861a16-367c-46eb-9db8-d20ecc93e38d"));

            migrationBuilder.DropColumn(
                name: "ProductRoles",
                table: "Products");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("7c55f045-7a18-4416-8120-81ed646cf627"), "Regular user role", "USER" },
                    { new Guid("8a066e9c-5fe5-418a-a933-eda7b4377c9b"), "Administrator role", "ADMIN" }
                });
        }
    }
}
