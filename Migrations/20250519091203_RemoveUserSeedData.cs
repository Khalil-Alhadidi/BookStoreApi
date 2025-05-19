using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookStoreApi.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "IsActive", "IsEmailConfirmed", "LastLoginAt", "PasswordHash", "PasswordSalt", "RefreshToken", "RefreshTokenExpiryTime", "Role", "UpdatedAt", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@bookstore.com", true, true, null, new byte[] { 1, 2, 3 }, new byte[] { 4, 5, 6 }, null, null, "Admin", null, "admin" },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "user@bookstore.com", true, false, null, new byte[] { 1, 2, 3 }, new byte[] { 4, 5, 6 }, null, null, "User", null, "user" }
                });
        }
    }
}
