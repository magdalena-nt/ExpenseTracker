using System.ComponentModel.DataAnnotations;
using expense_tracker.web.Models.Enums;
using Newtonsoft.Json;

namespace expense_tracker.web.Data.Entity;

public class TransactionEntity
{
    [Key]
    public int Id { get; set; }

    public decimal Value { get; set; }

    public Currency Currency { get; set; }

    [StringLength(16)]
    public string Name { get; set; }

    [StringLength(64)]
    public string? Note { get; set; }

    public DateTime Date { get; set; } = DateTime.Now;

    [StringLength(64)]
    public string? Location { get; set; }

    public Category Category { get; set; }

    public string UserId { get; set; }

    [JsonIgnore]
    public CustomUserEntity User { get; set; }
}