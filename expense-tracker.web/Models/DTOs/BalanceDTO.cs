using expense_tracker.web.Models.Enums;

namespace expense_tracker.web.Models.DTOs;

public class BalanceDTO
{
    public int Id { get; set; }
    public decimal Balance { get; set; }
    public Currency Currency { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal TotalIncome { get; set; }
}