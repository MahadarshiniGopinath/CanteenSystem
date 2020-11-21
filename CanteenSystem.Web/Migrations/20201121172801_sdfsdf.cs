using Microsoft.EntityFrameworkCore.Migrations;

namespace CanteenSystem.Web.Migrations
{
    public partial class sdfsdf : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParentMapping_Orders_OrderId",
                table: "ParentMapping");

            migrationBuilder.DropForeignKey(
                name: "FK_ParentMapping_Orders_OrderId1",
                table: "ParentMapping");

            migrationBuilder.DropIndex(
                name: "IX_ParentMapping_OrderId",
                table: "ParentMapping");

            migrationBuilder.DropIndex(
                name: "IX_ParentMapping_OrderId1",
                table: "ParentMapping");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "ParentMapping");

            migrationBuilder.DropColumn(
                name: "OrderId1",
                table: "ParentMapping");

            migrationBuilder.AlterColumn<int>(
                name: "DiscountId",
                table: "MealMenus",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "OrderId",
                table: "ParentMapping",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OrderId1",
                table: "ParentMapping",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DiscountId",
                table: "MealMenus",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParentMapping_OrderId",
                table: "ParentMapping",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ParentMapping_OrderId1",
                table: "ParentMapping",
                column: "OrderId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ParentMapping_Orders_OrderId",
                table: "ParentMapping",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ParentMapping_Orders_OrderId1",
                table: "ParentMapping",
                column: "OrderId1",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
