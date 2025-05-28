using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class FixPropertyGallery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "property_id",
                table: "PropertyGallery",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "user_id",
                table: "PropertyGallery",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_PropertyGallery_user_id",
                table: "PropertyGallery",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyGallery_User_user_id",
                table: "PropertyGallery",
                column: "user_id",
                principalTable: "User",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PropertyGallery_User_user_id",
                table: "PropertyGallery");

            migrationBuilder.DropIndex(
                name: "IX_PropertyGallery_user_id",
                table: "PropertyGallery");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "PropertyGallery");

            migrationBuilder.AlterColumn<long>(
                name: "property_id",
                table: "PropertyGallery",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }
    }
}
