using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LilamiBazzar.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addingLockoutEndcloumninUsertbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("6ed3838f-47aa-48d4-be57-713d96ff9d3f"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("b9a96a7e-981e-4dbc-9a8d-aee8ce3d949b"));

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("41c74959-ca99-47c9-a265-41a0926b6896"), "Regular user role", "USER" },
                    { new Guid("ed07219c-f75f-49b1-a4c0-71ca34f049f5"), "Administrator role", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("41c74959-ca99-47c9-a265-41a0926b6896"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("ed07219c-f75f-49b1-a4c0-71ca34f049f5"));

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("6ed3838f-47aa-48d4-be57-713d96ff9d3f"), "Regular user role", "USER" },
                    { new Guid("b9a96a7e-981e-4dbc-9a8d-aee8ce3d949b"), "Administrator role", "ADMIN" }
                });
        }
    }
}
