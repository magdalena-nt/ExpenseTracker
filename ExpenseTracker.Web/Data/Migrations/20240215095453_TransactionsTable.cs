using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace expense_tracker.web.Data.Migrations
{
    /// <inheritdoc />
    public partial class TransactionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Value = table.Column<decimal>(type: "TEXT", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(16)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(64)", nullable: true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(64)", nullable: true),
                    IsIncome = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions");
        }
    }
}
