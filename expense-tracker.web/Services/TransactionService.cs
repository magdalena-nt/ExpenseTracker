using expense_tracker.web.Data;
using expense_tracker.web.Data.Entity;
using expense_tracker.web.Models;
using Microsoft.EntityFrameworkCore;

namespace expense_tracker.web.Services;

public class TransactionService
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly BalanceService _balanceService;

    public TransactionService(ApplicationDbContext applicationDbContext, BalanceService balanceService)
    {
        _applicationDbContext = applicationDbContext;
        _balanceService = balanceService;
    }

    public async Task CreateTransaction(TransactionViewModel transactionVm, string? userId)
    {
        if (userId == null)
        {
            return;
        }

        var transaction = new TransactionEntity
        {
            Currency = transactionVm.Currency,
            UserId = userId,
            Category = transactionVm.Category,
            Date = transactionVm.Date,
            Location = transactionVm.Location,
            Name = transactionVm.Name,
            Note = transactionVm.Note,
            Value = transactionVm.Category > 0 ? transactionVm.Value : -transactionVm.Value
        };
        await _applicationDbContext.AddAsync(transaction);
        await _applicationDbContext.SaveChangesAsync();
        await _balanceService.UpdateBalance(transaction);
    }

    public async Task<TransactionEntity?> FindTransactionById(int id)
    {
        var transactionEntity = await _applicationDbContext.Transactions
            .FirstOrDefaultAsync(m => m.Id == id);
        return transactionEntity;
    }
}