using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Wallet.Migrations
{
    public partial class ImproveModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdDate",
                table: "Erc20Tokens");

            migrationBuilder.RenameColumn(
                name: "dateTime",
                table: "CustomEventLogs",
                newName: "WhenDateTime");

            migrationBuilder.AlterColumn<double>(
                name: "NumberOfTokenWasReceivedByAddress",
                table: "NotificationOptions",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<double>(
                name: "NumberOfTokenOrEtherWasReceived",
                table: "NotificationOptions",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<double>(
                name: "NumberOfTokenOrEtherThatWasSentTo",
                table: "NotificationOptions",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<double>(
                name: "NumberOfTokenOrEtherThatWasSentFrom",
                table: "NotificationOptions",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<double>(
                name: "NumberOfContractTokenWasSent",
                table: "NotificationOptions",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WhenDateTime",
                table: "CustomEventLogs",
                newName: "dateTime");

            migrationBuilder.AlterColumn<int>(
                name: "NumberOfTokenWasReceivedByAddress",
                table: "NotificationOptions",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<int>(
                name: "NumberOfTokenOrEtherWasReceived",
                table: "NotificationOptions",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<int>(
                name: "NumberOfTokenOrEtherThatWasSentTo",
                table: "NotificationOptions",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<int>(
                name: "NumberOfTokenOrEtherThatWasSentFrom",
                table: "NotificationOptions",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<int>(
                name: "NumberOfContractTokenWasSent",
                table: "NotificationOptions",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdDate",
                table: "Erc20Tokens",
                nullable: true);
        }
    }
}
