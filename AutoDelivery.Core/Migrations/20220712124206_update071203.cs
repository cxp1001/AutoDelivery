using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoDelivery.Core.Migrations
{
    public partial class update071203 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Users_UserAccountId",
                table: "Products");

            migrationBuilder.AlterColumn<int>(
                name: "UserAccountId",
                table: "Products",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Users_UserAccountId",
                table: "Products",
                column: "UserAccountId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Users_UserAccountId",
                table: "Products");

            migrationBuilder.AlterColumn<int>(
                name: "UserAccountId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Users_UserAccountId",
                table: "Products",
                column: "UserAccountId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
