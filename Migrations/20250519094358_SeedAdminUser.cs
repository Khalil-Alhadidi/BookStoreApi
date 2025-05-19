using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStoreApi.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "IsActive", "IsEmailConfirmed", "LastLoginAt", "PasswordHash", "PasswordSalt", "RefreshToken", "RefreshTokenExpiryTime", "Role", "UpdatedAt", "Username" },
                values: new object[] { 1, new DateTime(2025, 5, 19, 9, 43, 57, 298, DateTimeKind.Utc).AddTicks(7545), "admin@example.com", true, false, null, new byte[] { 131, 225, 85, 0, 155, 118, 196, 185, 9, 54, 242, 90, 236, 166, 32, 21, 184, 154, 92, 62, 180, 235, 231, 90, 58, 25, 249, 67, 192, 103, 25, 15, 37, 32, 84, 133, 135, 32, 133, 180, 92, 15, 71, 41, 173, 93, 75, 159, 60, 23, 157, 172, 210, 230, 207, 246, 248, 1, 100, 45, 52, 117, 102, 66 }, new byte[] { 77, 121, 83, 117, 112, 101, 114, 83, 101, 99, 114, 101, 116, 65, 112, 112, 76, 101, 118, 101, 108, 83, 97, 108, 116, 86, 97, 108, 117, 101, 49, 50, 51, 33, 64, 35, 50, 48, 50, 52 }, null, null, "Admin", null, "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
