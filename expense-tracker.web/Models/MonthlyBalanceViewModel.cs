namespace expense_tracker.web.Models;

public class MonthlyBalanceViewModel
{
    public int Year { get; set; }
    public int Month { get; set; }
    public IList<BalanceByCurrency> BalancesByCurrency { get; set; }
}

public class BalanceByCurrency
{
    public string Currency { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal Balance { get; set; }
    public bool IsInRed => Balance < 0;
}