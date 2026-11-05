using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace onlinelearningEF03.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Name", "ParentCategoryId" },
                values: new object[,]
                {
                    { 1, "All programming topics", "Programming", null },
                    { 4, null, "Design", null },
                    { 7, "Empty category (safe to delete)", "Archive", null }
                });

            migrationBuilder.InsertData(
                table: "Instructors",
                columns: new[] { "Id", "Bio", "Email", "FullName", "JoinedDate" },
                values: new object[,]
                {
                    { 1, "Senior .NET developer and trainer.", "ahmed.kamal@learnhub.com", "Ahmed Kamal", new DateTime(2022, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "Database architect with 10+ years of experience.", "mona.fathy@learnhub.com", "Mona Fathy", new DateTime(2022, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, null, "youssef.nabil@learnhub.com", "Youssef Nabil", new DateTime(2023, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Name", "ParentCategoryId" },
                values: new object[,]
                {
                    { 2, "Server-side development", "Backend", 1 },
                    { 5, "User interface and experience", "UI/UX", 4 },
                    { 6, "Client-side development", "Frontend", 1 }
                });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Id", "CategoryId", "Description", "InstructorId", "IsPublished", "Level", "Price", "PublishedDate", "Title" },
                values: new object[] { 1, 1, "Learn C# from scratch.", 1, true, "Beginner", 49.99m, new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "C# Fundamentals" });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Name", "ParentCategoryId" },
                values: new object[] { 3, "ASP.NET and .NET web", "ASP.NET", 2 });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Id", "CategoryId", "Description", "InstructorId", "IsPublished", "Level", "Price", "PublishedDate", "Title" },
                values: new object[,]
                {
                    { 3, 2, "Deep dive into EF Core.", 2, true, "Advanced", 119.99m, new DateTime(2024, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Advanced EF Core" },
                    { 4, 2, null, 2, false, "Beginner", 59.00m, null, "SQL Server Essentials" },
                    { 5, 6, "ES6 and beyond.", 3, true, "Intermediate", 74.50m, new DateTime(2024, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Modern JavaScript" },
                    { 6, 5, "Design principles.", 3, false, "Beginner", 39.00m, null, "UI/UX Design Basics" },
                    { 2, 3, "Build REST APIs.", 1, true, "Intermediate", 89.99m, new DateTime(2024, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "ASP.NET Core Web API" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Instructors",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Instructors",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Instructors",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
