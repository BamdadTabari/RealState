using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class FixDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Blog_short_description",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "short_description",
                table: "Blog");

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "Blog",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "description",
                table: "Blog");

            migrationBuilder.AddColumn<string>(
                name: "short_description",
                table: "Blog",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Blog_short_description",
                table: "Blog",
                column: "short_description");
        }
    }
}
