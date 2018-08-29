using Microsoft.EntityFrameworkCore.Migrations;

namespace Wallet.Migrations
{
    public partial class UpdateDecimalPrecision : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TokensSent",
                table: "TokenHolders",
                type: "decimal(18, 10)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "TokensReceived",
                table: "TokenHolders",
                type: "decimal(18, 10)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "TokenHolders",
                type: "decimal(18, 10)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "Erc20Tokens",
                type: "decimal(18, 10)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "AmountOfToken",
                table: "CustomEventLogs",
                type: "decimal(18, 10)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "DecimalValue",
                table: "BlockChainTransactions",
                type: "decimal(18, 10)",
                nullable: false,
                oldClrType: typeof(decimal));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TokensSent",
                table: "TokenHolders",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 10)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TokensReceived",
                table: "TokenHolders",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 10)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "TokenHolders",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 10)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "Erc20Tokens",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 10)");

            migrationBuilder.AlterColumn<decimal>(
                name: "AmountOfToken",
                table: "CustomEventLogs",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 10)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DecimalValue",
                table: "BlockChainTransactions",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 10)");
        }
    }
}
