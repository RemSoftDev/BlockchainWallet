using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Wallet.Migrations
{
    public partial class customerTrans : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomTransaction",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsSuccess = table.Column<bool>(nullable: false),
                    From = table.Column<string>(nullable: true),
                    To = table.Column<string>(nullable: true),
                    Hash = table.Column<string>(nullable: true),
                    DecimalValue = table.Column<decimal>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    What = table.Column<string>(nullable: true),
                    ContractAddress = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomTransaction", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomTransaction");
        }
    }
}
