using Microsoft.EntityFrameworkCore.Migrations;

namespace CanteenSystem.Web.Migrations
{
    public partial class s : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserProfileId",
                table: "Cart",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Cart_UserProfileId",
                table: "Cart",
                column: "UserProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_UserProfiles_UserProfileId",
                table: "Cart",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cart_UserProfiles_UserProfileId",
                table: "Cart");

            migrationBuilder.DropIndex(
                name: "IX_Cart_UserProfileId",
                table: "Cart");

            migrationBuilder.DropColumn(
                name: "UserProfileId",
                table: "Cart");
        }
    }
}
