﻿using expense_tracker.web.Models.Enums;
using Newtonsoft.Json;

namespace expense_tracker.web.Data.Entity;

public class BalanceEntity
{
    public int Id { get; set; }
    public string UserId { get; set; }

    [JsonIgnore]
    public CustomUserEntity User { get; set; }

    public Currency Currency { get; set; }
    public decimal Balance { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal TotalIncome { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public int Quantity { get; set; }
}