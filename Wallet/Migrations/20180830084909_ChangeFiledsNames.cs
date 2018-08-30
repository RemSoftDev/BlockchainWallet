using Microsoft.EntityFrameworkCore.Migrations;

namespace Wallet.Migrations
{
    public partial class ChangeFiledsNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "To",
                table: "BlockChainTransactions",
                newName: "ToAddress");

            migrationBuilder.RenameColumn(
                name: "From",
                table: "BlockChainTransactions",
                newName: "FromAddress");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ToAddress",
                table: "BlockChainTransactions",
                newName: "To");

            migrationBuilder.RenameColumn(
                name: "FromAddress",
                table: "BlockChainTransactions",
                newName: "From");
        }
    }
}
