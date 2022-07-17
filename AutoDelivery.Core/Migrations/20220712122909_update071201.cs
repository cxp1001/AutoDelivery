using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoDelivery.Core.Migrations
{
    public partial class update071201 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MailContentId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_MailContentId",
                table: "Products",
                column: "MailContentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_MailContents_MailContentId",
                table: "Products",
                column: "MailContentId",
                principalTable: "MailContents",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_MailContents_MailContentId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_MailContentId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "MailContentId",
                table: "Products");
        }
    }
}
