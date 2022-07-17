using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoDelivery.Core.Migrations
{
    public partial class update0712 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MailContents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MailTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentBegin = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentEnding = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailContents", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MailContents");
        }
    }
}
