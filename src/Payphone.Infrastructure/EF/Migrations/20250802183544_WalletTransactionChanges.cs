using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payphone.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class WalletTransactionChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransactions_Wallets_WalletId",
                table: "WalletTransactions");

            migrationBuilder.RenameColumn(
                name: "WalletId",
                table: "WalletTransactions",
                newName: "FromWalletId");

            migrationBuilder.RenameIndex(
                name: "IX_WalletTransactions_WalletId",
                table: "WalletTransactions",
                newName: "IX_WalletTransactions_FromWalletId");

            migrationBuilder.AddColumn<decimal>(
                name: "CurrentWalletBalance",
                table: "WalletTransactions",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ToWalletId",
                table: "WalletTransactions",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_ToWalletId",
                table: "WalletTransactions",
                column: "ToWalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransactions_Wallets_FromWalletId",
                table: "WalletTransactions",
                column: "FromWalletId",
                principalTable: "Wallets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransactions_Wallets_ToWalletId",
                table: "WalletTransactions",
                column: "ToWalletId",
                principalTable: "Wallets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransactions_Wallets_FromWalletId",
                table: "WalletTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransactions_Wallets_ToWalletId",
                table: "WalletTransactions");

            migrationBuilder.DropIndex(
                name: "IX_WalletTransactions_ToWalletId",
                table: "WalletTransactions");

            migrationBuilder.DropColumn(
                name: "CurrentWalletBalance",
                table: "WalletTransactions");

            migrationBuilder.DropColumn(
                name: "ToWalletId",
                table: "WalletTransactions");

            migrationBuilder.RenameColumn(
                name: "FromWalletId",
                table: "WalletTransactions",
                newName: "WalletId");

            migrationBuilder.RenameIndex(
                name: "IX_WalletTransactions_FromWalletId",
                table: "WalletTransactions",
                newName: "IX_WalletTransactions_WalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransactions_Wallets_WalletId",
                table: "WalletTransactions",
                column: "WalletId",
                principalTable: "Wallets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
