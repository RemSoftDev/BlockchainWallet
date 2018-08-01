using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Wallet.Migrations
{
    public partial class AddTokenHoldersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TokenHolders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Address = table.Column<string>(nullable: true),
                    Quantity = table.Column<decimal>(nullable: false),
                    TokensSent = table.Column<decimal>(nullable: false),
                    TokensReceived = table.Column<decimal>(nullable: false),
                    GeneralTransactionsNumber = table.Column<int>(nullable: false),
                    SentTransactionsNumber = table.Column<int>(nullable: false),
                    ReceivedTransactionsNumber = table.Column<int>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false),
                    ERC20TokenId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenHolders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TokenHolders_Erc20Tokens_ERC20TokenId",
                        column: x => x.ERC20TokenId,
                        principalTable: "Erc20Tokens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TokenHolders_ERC20TokenId",
                table: "TokenHolders",
                column: "ERC20TokenId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TokenHolders");
        }
    }
}
