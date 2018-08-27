using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Wallet.Migrations
{
    public partial class AddCHainBlock : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChainBlock",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Number = table.Column<int>(nullable: false),
                    TimeStamp = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChainBlock", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChainTransaction",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    From = table.Column<string>(nullable: true),
                    To = table.Column<string>(nullable: true),
                    Value = table.Column<decimal>(nullable: false),
                    IsSuccess = table.Column<bool>(nullable: false),
                    BlockNumber = table.Column<int>(nullable: false),
                    ChainBlockId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChainTransaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChainTransaction_ChainBlock_ChainBlockId",
                        column: x => x.ChainBlockId,
                        principalTable: "ChainBlock",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChainTransaction_ChainBlockId",
                table: "ChainTransaction",
                column: "ChainBlockId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChainTransaction");

            migrationBuilder.DropTable(
                name: "ChainBlock");
        }
    }
}
