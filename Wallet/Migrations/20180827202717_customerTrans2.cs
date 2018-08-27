using Microsoft.EntityFrameworkCore.Migrations;

namespace Wallet.Migrations
{
    public partial class customerTrans2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BlockNumber",
                table: "CustomTransaction",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlockNumber",
                table: "CustomTransaction");
        }
    }
}
