using expense_tracker.web.Data.Entity;
using expense_tracker.web.Models;
using expense_tracker.web.Models.DTOs;

namespace expense_tracker.web.Services.Mappers;

public static class BalanceMapper
{
    public static BalanceDTO MapBalanceDTO(BalanceEntity balanceEntity)
    {
        return new BalanceDTO
        {
            Id = balanceEntity.Id,
            Balance = balanceEntity.Balance,
            Currency = balanceEntity.Currency,
            Month = balanceEntity.Month,
            Year = balanceEntity.Year,
            TotalExpenses = balanceEntity.TotalExpenses,
            TotalIncome = balanceEntity.TotalIncome
        };
    }
}