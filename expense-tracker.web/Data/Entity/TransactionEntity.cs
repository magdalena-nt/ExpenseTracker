using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace expense_tracker.web.Data.Entity;

public class TransactionEntity
{
    [Key]
    public int Id { get; set; }

    public decimal Value { get; set; }
    
    [Column(TypeName = "nvarchar(3)")]
    public string Currency { get; set; }

    [Column(TypeName = "nvarchar(16)")]
    public string Name { get; set; }

    [Column(TypeName = "nvarchar(64)")]
    public string? Note { get; set; }

    public DateTime Date { get; set; } = DateTime.Now;

    [Column(TypeName = "nvarchar(64)")]
    public string? Location { get; set; }

    public bool IsIncome { get; set; }
}