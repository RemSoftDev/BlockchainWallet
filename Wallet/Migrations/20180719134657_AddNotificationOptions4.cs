using Microsoft.EntityFrameworkCore.Migrations;

namespace Wallet.Migrations
{
    public partial class AddNotificationOptions4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WhenTokenIsSent",
                table: "NotificationOptions",
                newName: "WhenTokenOrEtherIsSent");

            migrationBuilder.RenameColumn(
                name: "WhenTokenIsReceived",
                table: "NotificationOptions",
                newName: "WhenTokenOrEtherIsReceived");

            migrationBuilder.RenameColumn(
                name: "WhenNumberOfTokenWasSent",
                table: "NotificationOptions",
                newName: "WhenNumberOfTokenOrEtherWasSent");

            migrationBuilder.RenameColumn(
                name: "WhenNumberOfTokenWasReceived",
                table: "NotificationOptions",
                newName: "WhenNumberOfTokenOrEtherWasReceived");

            migrationBuilder.RenameColumn(
                name: "TokenWasReceivedName",
                table: "NotificationOptions",
                newName: "TokenOrEtherWasReceivedName");

            migrationBuilder.RenameColumn(
                name: "TokenSentName",
                table: "NotificationOptions",
                newName: "TokenOrEtherSentName");

            migrationBuilder.RenameColumn(
                name: "TokenReceivedName",
                table: "NotificationOptions",
                newName: "TokenOrEtherReceivedName");

            migrationBuilder.RenameColumn(
                name: "NumberOfTokenWasSentName",
                table: "NotificationOptions",
                newName: "NumberOfTokenOrEtherWasSentName");

            migrationBuilder.RenameColumn(
                name: "NumberOfTokenWasReceived",
                table: "NotificationOptions",
                newName: "NumberOfTokenOrEtherWasReceived");

            migrationBuilder.RenameColumn(
                name: "NumberOfTokenThatWasSentTo",
                table: "NotificationOptions",
                newName: "NumberOfTokenOrEtherThatWasSentTo");

            migrationBuilder.RenameColumn(
                name: "NumberOfTokenThatWasSentFrom",
                table: "NotificationOptions",
                newName: "NumberOfTokenOrEtherThatWasSentFrom");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WhenTokenOrEtherIsSent",
                table: "NotificationOptions",
                newName: "WhenTokenIsSent");

            migrationBuilder.RenameColumn(
                name: "WhenTokenOrEtherIsReceived",
                table: "NotificationOptions",
                newName: "WhenTokenIsReceived");

            migrationBuilder.RenameColumn(
                name: "WhenNumberOfTokenOrEtherWasSent",
                table: "NotificationOptions",
                newName: "WhenNumberOfTokenWasSent");

            migrationBuilder.RenameColumn(
                name: "WhenNumberOfTokenOrEtherWasReceived",
                table: "NotificationOptions",
                newName: "WhenNumberOfTokenWasReceived");

            migrationBuilder.RenameColumn(
                name: "TokenOrEtherWasReceivedName",
                table: "NotificationOptions",
                newName: "TokenWasReceivedName");

            migrationBuilder.RenameColumn(
                name: "TokenOrEtherSentName",
                table: "NotificationOptions",
                newName: "TokenSentName");

            migrationBuilder.RenameColumn(
                name: "TokenOrEtherReceivedName",
                table: "NotificationOptions",
                newName: "TokenReceivedName");

            migrationBuilder.RenameColumn(
                name: "NumberOfTokenOrEtherWasSentName",
                table: "NotificationOptions",
                newName: "NumberOfTokenWasSentName");

            migrationBuilder.RenameColumn(
                name: "NumberOfTokenOrEtherWasReceived",
                table: "NotificationOptions",
                newName: "NumberOfTokenWasReceived");

            migrationBuilder.RenameColumn(
                name: "NumberOfTokenOrEtherThatWasSentTo",
                table: "NotificationOptions",
                newName: "NumberOfTokenThatWasSentTo");

            migrationBuilder.RenameColumn(
                name: "NumberOfTokenOrEtherThatWasSentFrom",
                table: "NotificationOptions",
                newName: "NumberOfTokenThatWasSentFrom");
        }
    }
}
