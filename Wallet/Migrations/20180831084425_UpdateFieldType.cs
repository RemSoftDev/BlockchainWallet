using Microsoft.EntityFrameworkCore.Migrations;

namespace Wallet.Migrations
{
    public partial class UpdateFieldType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "DecimalValue",
                table: "BlockChainTransactions",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(38, 5)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "DecimalValue",
                table: "BlockChainTransactions",
                type: "decimal(38, 5)",
                nullable: false,
                oldClrType: typeof(double));
        }
    }
}
