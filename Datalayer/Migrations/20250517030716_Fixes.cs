using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class Fixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "gallery",
                table: "Property",
                newName: "video_caption");

            migrationBuilder.AddColumn<string>(
                name: "video",
                table: "Property",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "PropertyGallery",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    picture = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    alt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    property_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    slug = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyGallery", x => x.id);
                    table.ForeignKey(
                        name: "FK_PropertyGallery_Property_property_id",
                        column: x => x.property_id,
                        principalTable: "Property",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PropertyGallery_property_id",
                table: "PropertyGallery",
                column: "property_id");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyGallery_slug",
                table: "PropertyGallery",
                column: "slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PropertyGallery");

            migrationBuilder.DropColumn(
                name: "video",
                table: "Property");

            migrationBuilder.RenameColumn(
                name: "video_caption",
                table: "Property",
                newName: "gallery");
        }
    }
}
