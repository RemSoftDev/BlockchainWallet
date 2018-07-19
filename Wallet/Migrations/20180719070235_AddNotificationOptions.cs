using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Wallet.Migrations
{
    public partial class AddNotificationOptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Action",
                table: "UserWatchlist");

            migrationBuilder.CreateTable(
                name: "NotificationOptions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsWithoutNotifications = table.Column<bool>(nullable: false),
                    WhenTokenIsSent = table.Column<bool>(nullable: false),
                    TokenSentName = table.Column<string>(nullable: true),
                    WhenAnythingWasSent = table.Column<bool>(nullable: false),
                    WhenNumberOfTokenWasSent = table.Column<bool>(nullable: false),
                    NumberOfTokenThatWasSent = table.Column<int>(nullable: false),
                    NumberOfTokenWasSentName = table.Column<string>(nullable: true),
                    WhenTokenIsReceived = table.Column<bool>(nullable: false),
                    TokenReceivedName = table.Column<string>(nullable: true),
                    WhenNumberOfTokenWasReceived = table.Column<bool>(nullable: false),
                    NumberOfTokenWasReceived = table.Column<int>(nullable: false),
                    TokenWasReceivedName = table.Column<string>(nullable: true),
                    WhenNumberOfContractTokenWasSent = table.Column<bool>(nullable: false),
                    NumberOfContractTokenWasSent = table.Column<int>(nullable: false),
                    WhenNumberOfContractWasReceivedByAddress = table.Column<bool>(nullable: false),
                    NumberOfTokenWasReceivedByAddress = table.Column<int>(nullable: false),
                    AddressThatReceivedNumberOfToken = table.Column<string>(nullable: true),
                    UserWatchlistId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationOptions_UserWatchlist_UserWatchlistId",
                        column: x => x.UserWatchlistId,
                        principalTable: "UserWatchlist",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationOptions_UserWatchlistId",
                table: "NotificationOptions",
                column: "UserWatchlistId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationOptions");

            migrationBuilder.AddColumn<int>(
                name: "Action",
                table: "UserWatchlist",
                nullable: false,
                defaultValue: 0);
        }
    }
}
