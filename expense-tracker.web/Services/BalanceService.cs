using expense_tracker.web.Data;
using expense_tracker.web.Data.Entity;
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
        var currentBalance = await _applicationDbContext.Balances
            .FirstOrDefaultAsync(b =>
                b.UserId.Equals(transactionEntity.UserId) && b.Currency == transactionEntity.Currency);
        if (currentBalance != null)
        {
            currentBalance.Balance += transactionEntity.Value;
            currentBalance.Quantity++;
            _applicationDbContext.Update(currentBalance);
        }
        else
        {
            currentBalance = new BalanceEntity
            {
                Balance = transactionEntity.Value,
                Currency = transactionEntity.Currency,
                Quantity = 1,
                UserId = transactionEntity.UserId
            };
            await _applicationDbContext.AddAsync(currentBalance);
        }

        await _applicationDbContext.SaveChangesAsync();
    }
}