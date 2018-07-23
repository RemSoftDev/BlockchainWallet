using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Wallet.Migrations
{
    public partial class UpdateERC20TokenTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Erc20Tokens_SmartContracts_SmartContractId",
                table: "Erc20Tokens");

            migrationBuilder.DropTable(
                name: "SmartContracts");

            migrationBuilder.DropIndex(
                name: "IX_Erc20Tokens_SmartContractId",
                table: "Erc20Tokens");

            migrationBuilder.RenameColumn(
                name: "SmartContractId",
                table: "Erc20Tokens",
                newName: "WalletsCount");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Erc20Tokens",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Erc20Tokens",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Erc20Tokens",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TransactionsCount",
                table: "Erc20Tokens",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "WebSiteLink",
                table: "Erc20Tokens",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Erc20Tokens");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Erc20Tokens");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Erc20Tokens");

            migrationBuilder.DropColumn(
                name: "TransactionsCount",
                table: "Erc20Tokens");

            migrationBuilder.DropColumn(
                name: "WebSiteLink",
                table: "Erc20Tokens");

            migrationBuilder.RenameColumn(
                name: "WalletsCount",
                table: "Erc20Tokens",
                newName: "SmartContractId");

            migrationBuilder.CreateTable(
                name: "SmartContracts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    TransactionsCount = table.Column<int>(nullable: false),
                    WalletsCount = table.Column<int>(nullable: false),
                    WebSiteLink = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmartContracts", x => x.Id);
                });

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
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
