using expense_tracker.web.Models.Enums;

namespace expense_tracker.web.Models.DTOs;

public class TransactionDTO
{
    public int Id { get; set; }
    public decimal Value { get; set; }

    public string Currency { get; set; }

    public string Name { get; set; }

    public string? Note { get; set; }

    public DateTime Date { get; set; } = DateTime.Now;

    public string? Location { get; set; }

    public string Category { get; set; }
}