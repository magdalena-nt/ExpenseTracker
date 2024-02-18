namespace expense_tracker.web.Models;

public class MonthlyBalanceViewModel
{
    public int Year { get; set; }
    public int Month { get; set; }
    public IList<BalanceByCurrencyViewModel> BalancesByCurrency { get; set; }
}