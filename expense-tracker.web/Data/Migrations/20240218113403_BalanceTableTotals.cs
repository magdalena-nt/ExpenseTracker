using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace expense_tracker.web.Data.Migrations
{
    /// <inheritdoc />
    public partial class BalanceTableTotals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Month",
                table: "Balances",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalExpenses",
                table: "Balances",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalIncome",
                table: "Balances",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Balances",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Month",
                table: "Balances");

            migrationBuilder.DropColumn(
                name: "TotalExpenses",
                table: "Balances");

            migrationBuilder.DropColumn(
                name: "TotalIncome",
                table: "Balances");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Balances");
        }
    }
}
