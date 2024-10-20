using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LilamiBazzar.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReviewTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "IsLiked",
                table: "Reviews");

            migrationBuilder.RenameColumn(
                name: "Rating",
                table: "Reviews",
                newName: "PositiveRating");

            migrationBuilder.AddColumn<int>(
                name: "NegativeRating",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("171d7b89-e6de-4f75-b24f-a54b4a121a21"), "Regular user role", "USER" },
                    { new Guid("cc68dacf-4d1e-4ad3-acd4-5fcf72a477ea"), "Administrator role", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "NegativeRating",
                table: "Reviews");

            migrationBuilder.RenameColumn(
                name: "PositiveRating",
                table: "Reviews",
                newName: "Rating");

            migrationBuilder.AddColumn<bool>(
                name: "IsLiked",
                table: "Reviews",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("195e1adf-d35f-4a96-83aa-3d96df97aa95"), "Administrator role", "ADMIN" },
                    { new Guid("92c73f3e-16f1-4d6b-8ec5-ab423daef368"), "Regular user role", "USER" }
                });
        }
    }
}
