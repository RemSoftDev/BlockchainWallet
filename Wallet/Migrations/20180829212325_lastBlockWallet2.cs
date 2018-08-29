using Microsoft.EntityFrameworkCore.Migrations;

namespace Wallet.Migrations
{
    public partial class lastBlockWallet2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Block",
                table: "LastWalletBlocks",
                newName: "BlockStart");

            migrationBuilder.AddColumn<int>(
                name: "BlockEnd",
                table: "LastWalletBlocks",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlockEnd",
                table: "LastWalletBlocks");

            migrationBuilder.RenameColumn(
                name: "BlockStart",
                table: "LastWalletBlocks",
                newName: "Block");
        }
    }
}
