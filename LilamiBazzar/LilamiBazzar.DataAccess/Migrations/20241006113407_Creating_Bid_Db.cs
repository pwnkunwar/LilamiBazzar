using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LilamiBazzar.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Creating_Bid_Db : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("bf0a292b-eef0-4947-b601-a37a4797ccea"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("e0f5f5bc-9745-462e-b8e3-d81dbaddb00d"));

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("4a60d11e-3977-4bd3-b397-d72e6d82190a"), "Administrator role", "ADMIN" },
                    { new Guid("9b4e16bf-7fd9-4bc1-8263-7dbbc543be5d"), "Regular user role", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("4a60d11e-3977-4bd3-b397-d72e6d82190a"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("9b4e16bf-7fd9-4bc1-8263-7dbbc543be5d"));

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("bf0a292b-eef0-4947-b601-a37a4797ccea"), "Regular user role", "USER" },
                    { new Guid("e0f5f5bc-9745-462e-b8e3-d81dbaddb00d"), "Administrator role", "ADMIN" }
                });
        }
    }
}
