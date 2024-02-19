using expense_tracker.web.Data;
using expense_tracker.web.Data.Entity;
using expense_tracker.web.Models;
using expense_tracker.web.Models.DTOs;
using expense_tracker.web.Models.Enums;
using expense_tracker.web.Services.Mappers;
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

    public async Task<IList<BalanceDTO>> FindAllBalancesByYearMonth(int year, int month)
    {
        var balanceEntities = await _applicationDbContext.Balances
            .Where(b => b.Year == year && b.Month == month).ToListAsync();
        var balances = balanceEntities.Select(BalanceMapper.MapBalanceDTO).ToList();
        return balances;
    }

    public async Task<Dictionary<Currency, decimal>> FindBalanceSumByYearMonth(int year, int month)
    {
        var dictionary = (await FindAllBalancesByYearMonth(year, month)).GroupBy(b => b.Currency)
            .ToDictionary(g => g.Key, g => g.Sum(b => b.Balance)).ToDictionary();
        return dictionary;
    }

    public async Task<IEnumerable<KeyValuePair<Currency, decimal>>> FindBalanceSumByYearMonthCurrency(int year, int month, string currency)
    {
        var dictionary =
            (await FindBalanceSumByYearMonth(year, month)).Where(g => g.Key.Equals(Enum.Parse<Currency>(currency))).ToDictionary();

        return dictionary;
    }
}