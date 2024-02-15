using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace expense_tracker.web.Data.Migrations
{
    /// <inheritdoc />
    public partial class TransactionUserFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserEntityId",
                table: "Transactions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Transactions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Transactions");
        }
    }
}
