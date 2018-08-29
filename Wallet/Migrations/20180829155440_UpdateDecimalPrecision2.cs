using Microsoft.EntityFrameworkCore.Migrations;

namespace Wallet.Migrations
{
    public partial class UpdateDecimalPrecision2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TokensSent",
                table: "TokenHolders",
                type: "decimal(28, 10)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 10)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TokensReceived",
                table: "TokenHolders",
                type: "decimal(28, 10)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 10)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "TokenHolders",
                type: "decimal(28, 10)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 10)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "Erc20Tokens",
                type: "decimal(28, 10)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 10)");

            migrationBuilder.AlterColumn<decimal>(
                name: "AmountOfToken",
                table: "CustomEventLogs",
                type: "decimal(28, 10)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 10)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DecimalValue",
                table: "BlockChainTransactions",
                type: "decimal(28, 10)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 10)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TokensSent",
                table: "TokenHolders",
                type: "decimal(18, 10)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 10)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TokensReceived",
                table: "TokenHolders",
                type: "decimal(18, 10)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 10)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "TokenHolders",
                type: "decimal(18, 10)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 10)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "Erc20Tokens",
                type: "decimal(18, 10)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 10)");

            migrationBuilder.AlterColumn<decimal>(
                name: "AmountOfToken",
                table: "CustomEventLogs",
                type: "decimal(18, 10)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 10)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DecimalValue",
                table: "BlockChainTransactions",
                type: "decimal(18, 10)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 10)");
        }
    }
}
