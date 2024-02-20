using expense_tracker.web.Models.Enums;

namespace expense_tracker.web.Models;

public class TransactionViewModel
{
    public int Id { get; set; }
    public decimal Value { get; set; }

    public Currency Currency { get; set; }

    public string Name { get; set; }

    public string? Note { get; set; }

    public DateTime Date { get; set; } = DateTime.Now;

    public string? Location { get; set; }

    public Category Category { get; set; }
}