using expense_tracker.web.Data;
using expense_tracker.web.Data.Entity;
using expense_tracker.web.Models;
using Microsoft.EntityFrameworkCore;

namespace expense_tracker.web.Services;

public class BalanceService
{
    private readonly ApplicationDbContext _applicationDbContext;

    public BalanceService(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task UpdateBalance(TransactionEntity transactionEntity)
    {
        var currentBalance = await GetCurrentBalance(transactionEntity);
        if (currentBalance != null)
        {
            {
                currentBalance.Quantity++;
                currentBalance.TotalIncome += transactionEntity.Value > 0 ? transactionEntity.Value : 0;
                currentBalance.TotalExpenses += transactionEntity.Value < 0 ? transactionEntity.Value : 0;
                currentBalance.Balance = currentBalance.TotalIncome + currentBalance.TotalExpenses;
                _applicationDbContext.Update(currentBalance);
            }
        }
        else
        {
            currentBalance = new BalanceEntity
            {
                Balance = transactionEntity.Value,
                Currency = transactionEntity.Currency,
                Quantity = 1,
                UserId = transactionEntity.UserId,
                TotalExpenses = transactionEntity.Value > 0 ? 0 : transactionEntity.Value,
                TotalIncome = transactionEntity.Value > 0 ? transactionEntity.Value : 0,
                Year = transactionEntity.Date.Year,
                Month = transactionEntity.Date.Month
            };
            await _applicationDbContext.AddAsync(currentBalance);
        }

        await _applicationDbContext.SaveChangesAsync();
    }

    private async Task<BalanceEntity?> GetCurrentBalance(TransactionEntity transactionEntity)
    {
        var currentBalance = await _applicationDbContext.Balances
            .FirstOrDefaultAsync(b =>
                b.UserId.Equals(transactionEntity.UserId) && b.Currency == transactionEntity.Currency &&
                b.Year == transactionEntity.Date.Year
                && b.Month == transactionEntity.Date.Month);
        return currentBalance;
    }

    public async Task ClearFromBalance(TransactionEntity transactionEntity)
    {
        var currentBalance = await GetCurrentBalance(transactionEntity);
        if (currentBalance != null)
        {
            currentBalance.Quantity++;
            currentBalance.TotalIncome -= transactionEntity.Value > 0 ? transactionEntity.Value : 0;
            currentBalance.TotalExpenses -= transactionEntity.Value < 0 ? transactionEntity.Value : 0;
            currentBalance.Balance = currentBalance.TotalIncome + currentBalance.TotalExpenses;
            _applicationDbContext.Update(currentBalance);
        }
    }

    public async Task<MonthlyBalanceViewModel> FindMonthlyBalanceByDateAndUser(int year, int month, string? userId)
    {
        var balanceEntities = await _applicationDbContext.Balances
            .Where(b => b.UserId.Equals(userId) && b.Year == year && b.Month == month).ToListAsync();
        var balancesByCurrency = balanceEntities.Select(entity => new BalanceByCurrencyViewModel
        {
            Balance = entity.Balance,
            Currency = entity.Currency.ToString(),
            TotalIncome = entity.TotalIncome,
            TotalExpenses = entity.TotalExpenses
        });

        var model = new MonthlyBalanceViewModel
        {
            Year = year,
            Month = month,
            BalancesByCurrency = balancesByCurrency.ToList()
        };

        return model;
    }
}