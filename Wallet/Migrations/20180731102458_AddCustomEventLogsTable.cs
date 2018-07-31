using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Wallet.Migrations
{
    public partial class AddCustomEventLogsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TokenHolder");

            migrationBuilder.CreateTable(
                name: "CustomEventLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    From = table.Column<string>(nullable: true),
                    To = table.Column<string>(nullable: true),
                    AmountOfToken = table.Column<decimal>(nullable: false),
                    dateTime = table.Column<DateTime>(nullable: false),
                    BlockNumber = table.Column<int>(nullable: false),
                    ERC20TokenId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomEventLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomEventLogs_Erc20Tokens_ERC20TokenId",
                        column: x => x.ERC20TokenId,
                        principalTable: "Erc20Tokens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomEventLogs_ERC20TokenId",
                table: "CustomEventLogs",
                column: "ERC20TokenId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomEventLogs");

            migrationBuilder.CreateTable(
                name: "TokenHolder",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Address = table.Column<string>(nullable: false),
                    ERC20TokenId = table.Column<int>(nullable: false),
                    GeneralTransactionsNumber = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    ReceivedTransactionsNumber = table.Column<int>(nullable: false),
                    SentTransactionsNumber = table.Column<int>(nullable: false),
                    TokensReceived = table.Column<int>(nullable: false),
                    TokensSent = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenHolder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TokenHolder_Erc20Tokens_ERC20TokenId",
                        column: x => x.ERC20TokenId,
                        principalTable: "Erc20Tokens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TokenHolder_ERC20TokenId",
                table: "TokenHolder",
                column: "ERC20TokenId");
        }
    }
}
