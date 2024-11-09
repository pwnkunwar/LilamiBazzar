using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LilamiBazzar.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ChangesonUsertbl2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("30c04b18-8093-40d6-b4bf-97d7dc9d9c3b"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("5d46702d-44e0-415d-b48a-d23e02fecd18"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "PasswordResetTokenExpires",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("156f9671-d994-4dc3-83b1-0b2b8ed43307"), "Administrator role", "ADMIN" },
                    { new Guid("a0961fe2-061a-4749-bd94-fe7afbeb5624"), "Regular user role", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("156f9671-d994-4dc3-83b1-0b2b8ed43307"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("a0961fe2-061a-4749-bd94-fe7afbeb5624"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "PasswordResetTokenExpires",
                table: "Users",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("30c04b18-8093-40d6-b4bf-97d7dc9d9c3b"), "Administrator role", "ADMIN" },
                    { new Guid("5d46702d-44e0-415d-b48a-d23e02fecd18"), "Regular user role", "USER" }
                });
        }
    }
}
