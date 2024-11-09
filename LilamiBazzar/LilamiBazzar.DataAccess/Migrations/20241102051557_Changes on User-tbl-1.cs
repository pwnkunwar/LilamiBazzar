using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LilamiBazzar.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ChangesonUsertbl1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("2e32f1ad-477c-4a64-892e-fe8fe5219a4e"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("6a79c7a4-cc02-400f-928d-b13b71dbaba3"));

            migrationBuilder.AddColumn<string>(
                name: "NewEmail",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("30c04b18-8093-40d6-b4bf-97d7dc9d9c3b"), "Administrator role", "ADMIN" },
                    { new Guid("5d46702d-44e0-415d-b48a-d23e02fecd18"), "Regular user role", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("30c04b18-8093-40d6-b4bf-97d7dc9d9c3b"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("5d46702d-44e0-415d-b48a-d23e02fecd18"));

            migrationBuilder.DropColumn(
                name: "NewEmail",
                table: "Users");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("2e32f1ad-477c-4a64-892e-fe8fe5219a4e"), "Administrator role", "ADMIN" },
                    { new Guid("6a79c7a4-cc02-400f-928d-b13b71dbaba3"), "Regular user role", "USER" }
                });
        }
    }
}
