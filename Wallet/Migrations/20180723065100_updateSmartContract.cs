using Microsoft.EntityFrameworkCore.Migrations;

namespace Wallet.Migrations
{
    public partial class updateSmartContract : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "SmartContracts");

            migrationBuilder.AddColumn<int>(
                name: "SmartContractId",
                table: "Erc20Tokens",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Erc20Tokens_SmartContractId",
                table: "Erc20Tokens",
                column: "SmartContractId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Erc20Tokens_SmartContracts_SmartContractId",
                table: "Erc20Tokens",
                column: "SmartContractId",
                principalTable: "SmartContracts",
                principalColumn: "SmartContractId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Erc20Tokens_SmartContracts_SmartContractId",
                table: "Erc20Tokens");

            migrationBuilder.DropIndex(
                name: "IX_Erc20Tokens_SmartContractId",
                table: "Erc20Tokens");

            migrationBuilder.DropColumn(
                name: "SmartContractId",
                table: "Erc20Tokens");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "SmartContracts",
                nullable: false,
                defaultValue: "");
        }
    }
}
