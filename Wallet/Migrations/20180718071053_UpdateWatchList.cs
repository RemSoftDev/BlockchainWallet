using Microsoft.EntityFrameworkCore.Migrations;

namespace Wallet.Migrations
{
    public partial class UpdateWatchList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Adress",
                table: "UserWatchlist");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserWatchlist");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "UserWatchlist",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserEmail",
                table: "UserWatchlist",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "UserWatchlist");

            migrationBuilder.DropColumn(
                name: "UserEmail",
                table: "UserWatchlist");

            migrationBuilder.AddColumn<string>(
                name: "Adress",
                table: "UserWatchlist",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "UserWatchlist",
                nullable: true);
        }
    }
}
