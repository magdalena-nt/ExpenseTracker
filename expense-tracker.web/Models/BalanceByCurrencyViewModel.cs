namespace expense_tracker.web.Models;

public class BalanceByCurrencyViewModel
{
    public string Currency { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal Balance { get; set; }
    public bool IsInRed => Balance < 0;
}