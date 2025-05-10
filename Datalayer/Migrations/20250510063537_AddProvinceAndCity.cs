using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddProvinceAndCity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Province",
                columns: new[] { "id", "created_at", "name", "slug", "updated_at" },
                values: new object[] { 1L, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "مازندران", "مازندران", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "City",
                columns: new[] { "id", "created_at", "name", "province_id", "slug", "updated_at" },
                values: new object[,]
                {
                    { 1L, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "ساری", 1L, "ساری", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2L, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "بابل", 1L, "بابل", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3L, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "آمل", 1L, "آمل", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4L, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "قائمشهر", 1L, "قائمشهر", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5L, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "تنکابن", 1L, "تنکابن", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6L, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "نوشهر", 1L, "نوشهر", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 7L, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "چالوس", 1L, "چالوس", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 8L, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "نور", 1L, "نور", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 9L, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "جویبار", 1L, "جویبار", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 10L, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "رامسر", 1L, "رامسر", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 11L, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "بهشهر", 1L, "بهشهر", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 12L, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "سوادکوه", 1L, "سوادکوه", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 13L, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "عباس‌آباد", 1L, "عباس-آباد", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 14L, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "فریدونکنار", 1L, "فریدونکنار", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 15L, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "محمودآباد", 1L, "محمودآباد", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 16L, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "نکا", 1L, "نكا", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 17L, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "سیمرغ", 1L, "سیمرغ", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 18L, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "گلوگاه", 1L, "گلوگاه", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 19L, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "شهمیرزاد", 1L, "شهمیرزاد", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 20L, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "کجور", 1L, "کجور", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 21L, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "کلاردشت", 1L, "کلاردشت", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 22L, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "املش", 1L, "املش", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 23L, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "میاندورود", 1L, "میاندورود", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "id",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "id",
                keyValue: 5L);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "id",
                keyValue: 6L);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "id",
                keyValue: 7L);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "id",
                keyValue: 8L);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "id",
                keyValue: 9L);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "id",
                keyValue: 10L);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "id",
                keyValue: 11L);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "id",
                keyValue: 12L);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "id",
                keyValue: 13L);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "id",
                keyValue: 14L);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "id",
                keyValue: 15L);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "id",
                keyValue: 16L);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "id",
                keyValue: 17L);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "id",
                keyValue: 18L);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "id",
                keyValue: 19L);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "id",
                keyValue: 20L);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "id",
                keyValue: 21L);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "id",
                keyValue: 22L);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "id",
                keyValue: 23L);

            migrationBuilder.DeleteData(
                table: "Province",
                keyColumn: "id",
                keyValue: 1L);
        }
    }
}
