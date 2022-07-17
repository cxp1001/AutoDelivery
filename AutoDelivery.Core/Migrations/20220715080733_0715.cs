using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoDelivery.Core.Migrations
{
    public partial class _0715 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentBegin",
                table: "MailContents");

            migrationBuilder.RenameColumn(
                name: "ContentEnding",
                table: "MailContents",
                newName: "MainContent");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MainContent",
                table: "MailContents",
                newName: "ContentEnding");

            migrationBuilder.AddColumn<string>(
                name: "ContentBegin",
                table: "MailContents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
