using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace expense_tracker.web.Data.Migrations
{
    /// <inheritdoc />
    public partial class TransactionsUserUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_AspNetUsers_UserEntityId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_UserEntityId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "UserEntityId",
                table: "Transactions");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserId",
                table: "Transactions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_AspNetUsers_UserId",
                table: "Transactions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_AspNetUsers_UserId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_UserId",
                table: "Transactions");

            migrationBuilder.AddColumn<string>(
                name: "UserEntityId",
                table: "Transactions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserEntityId",
                table: "Transactions",
                column: "UserEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_AspNetUsers_UserEntityId",
                table: "Transactions",
                column: "UserEntityId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
