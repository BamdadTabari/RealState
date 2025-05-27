using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class FixPlanOrderRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Order_plan_id",
                table: "Order");

            migrationBuilder.CreateIndex(
                name: "IX_Order_plan_id",
                table: "Order",
                column: "plan_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Order_plan_id",
                table: "Order");

            migrationBuilder.CreateIndex(
                name: "IX_Order_plan_id",
                table: "Order",
                column: "plan_id",
                unique: true);
        }
    }
}
