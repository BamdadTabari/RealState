using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class FixRelationInPropertyAndCity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Property_City_cityid",
                table: "Property");

            migrationBuilder.DropIndex(
                name: "IX_Property_cityid",
                table: "Property");

            migrationBuilder.DropColumn(
                name: "cityid",
                table: "Property");

            migrationBuilder.CreateIndex(
                name: "IX_Property_city_id",
                table: "Property",
                column: "city_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Property_City_city_id",
                table: "Property",
                column: "city_id",
                principalTable: "City",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Property_City_city_id",
                table: "Property");

            migrationBuilder.DropIndex(
                name: "IX_Property_city_id",
                table: "Property");

            migrationBuilder.AddColumn<long>(
                name: "cityid",
                table: "Property",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Property_cityid",
                table: "Property",
                column: "cityid");

            migrationBuilder.AddForeignKey(
                name: "FK_Property_City_cityid",
                table: "Property",
                column: "cityid",
                principalTable: "City",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
