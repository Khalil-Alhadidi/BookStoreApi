using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookStoreApi.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOldUsersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 3);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsEmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PasswordHash = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    PasswordSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "Author", "Price", "Title" },
                values: new object[,]
                {
                    { 1, "Andrew Hunt", 39.99m, "The Pragmatic Programmer" },
                    { 2, "Robert C. Martin", 29.99m, "Clean Code" },
                    { 3, "Eric Evans", 49.99m, "Domain-Driven Design" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "IsActive", "IsEmailConfirmed", "LastLoginAt", "PasswordHash", "PasswordSalt", "RefreshToken", "RefreshTokenExpiryTime", "Role", "UpdatedAt", "Username" },
                values: new object[] { 1, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@example.com", true, false, null, new byte[] { 131, 225, 85, 0, 155, 118, 196, 185, 9, 54, 242, 90, 236, 166, 32, 21, 184, 154, 92, 62, 180, 235, 231, 90, 58, 25, 249, 67, 192, 103, 25, 15, 37, 32, 84, 133, 135, 32, 133, 180, 92, 15, 71, 41, 173, 93, 75, 159, 60, 23, 157, 172, 210, 230, 207, 246, 248, 1, 100, 45, 52, 117, 102, 66 }, new byte[] { 77, 121, 83, 117, 112, 101, 114, 83, 101, 99, 114, 101, 116, 65, 112, 112, 76, 101, 118, 101, 108, 83, 97, 108, 116, 86, 97, 108, 117, 101, 49, 50, 51, 33, 64, 35, 50, 48, 50, 52 }, null, null, "Admin", null, "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }
    }
}
