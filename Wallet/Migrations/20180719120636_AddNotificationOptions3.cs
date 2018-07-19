using Microsoft.EntityFrameworkCore.Migrations;

namespace Wallet.Migrations
{
    public partial class AddNotificationOptions3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NumberOfTokenThatWasSent",
                table: "NotificationOptions",
                newName: "NumberOfTokenThatWasSentTo");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfTokenThatWasSentFrom",
                table: "NotificationOptions",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfTokenThatWasSentFrom",
                table: "NotificationOptions");

            migrationBuilder.RenameColumn(
                name: "NumberOfTokenThatWasSentTo",
                table: "NotificationOptions",
                newName: "NumberOfTokenThatWasSent");
        }
    }
}
