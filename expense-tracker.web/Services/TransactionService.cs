using expense_tracker.web.Data;
using expense_tracker.web.Data.Entity;
using expense_tracker.web.Models;
using expense_tracker.web.Services.Mappers;
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
            Id = transactionVm.Id,
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

    public TransactionViewModel? FindTransactionVmById(int id)
    {
        var transactionEntity = FindTransactionById(id).Result;
        if (transactionEntity != null)
        {
            return TransactionMapper.Map(transactionEntity);
        }

        return null;
    }

    public IEnumerable<TransactionViewModel> FindTransactionVMsByUser(string? userId)
    {
        var transactionEntities = _applicationDbContext.Transactions.Where(t => t.UserId.Equals(userId)).ToList();
        var transactionVMs = transactionEntities.Select(TransactionMapper.Map).ToList();

        return transactionVMs;
    }

    public async Task DeleteTransactionById(int id)
    {
        var transactionEntity = await FindTransactionById(id);
        if (transactionEntity != null)
        {
            await _balanceService.ClearFromBalance(transactionEntity);
            _applicationDbContext.Transactions.Remove(transactionEntity);
        }

        await _applicationDbContext.SaveChangesAsync();
    }

    public async Task EditTransactionById(int id, string userId, TransactionViewModel transactionViewModel)
    {
        var transactionEntity = await FindTransactionById(id);
        if (transactionEntity != null)
        {
            await _balanceService.ClearFromBalance(transactionEntity);
            transactionEntity.Currency = transactionViewModel.Currency;
            transactionEntity.Category = transactionViewModel.Category;
            transactionEntity.Date = transactionViewModel.Date;
            transactionEntity.Location = transactionViewModel.Location;
            transactionEntity.Name = transactionViewModel.Name;
            transactionEntity.Note = transactionViewModel.Note;
            transactionEntity.Value = transactionViewModel.Value;
            await _balanceService.UpdateBalance(transactionEntity);
        }

        await _applicationDbContext.SaveChangesAsync();
    }

    public IEnumerable<TransactionViewModel> FindIncomesByUser(string? userId)
    {
        return FindTransactionVMsByUser(userId).Where(t => t.Category > 0)
            .OrderByDescending(t => t.Date).ToList();
    }

    public IEnumerable<TransactionViewModel> FindExpensesByUser(string? userId)
    {
        return FindTransactionVMsByUser(userId).Where(t => t.Category < 0)
            .OrderByDescending(t => t.Date).ToList();
    }
}