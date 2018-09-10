using Microsoft.EntityFrameworkCore.Migrations;

namespace Wallet.Migrations
{
    public partial class AddTokenDecimalPlacesField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TokenDecimalPlaces",
                table: "UserWatchlist",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TokenDecimalPlaces",
                table: "UserWatchlist");
        }
    }
}
