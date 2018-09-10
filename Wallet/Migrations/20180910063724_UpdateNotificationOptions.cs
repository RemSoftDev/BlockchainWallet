using Microsoft.EntityFrameworkCore.Migrations;

namespace Wallet.Migrations
{
    public partial class UpdateNotificationOptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TokenReceivedDecimalPlaces",
                table: "NotificationOptions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TokenSentDecimalPlaces",
                table: "NotificationOptions",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TokenReceivedDecimalPlaces",
                table: "NotificationOptions");

            migrationBuilder.DropColumn(
                name: "TokenSentDecimalPlaces",
                table: "NotificationOptions");
        }
    }
}
