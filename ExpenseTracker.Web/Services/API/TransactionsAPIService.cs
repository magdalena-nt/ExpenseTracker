using expense_tracker.web.Data;
using expense_tracker.web.Models.DTOs;
using expense_tracker.web.Models.Enums;
using expense_tracker.web.Services.Mappers;
using Microsoft.EntityFrameworkCore;

namespace expense_tracker.web.Services.API;

public class TransactionsAPIService
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly BalanceService _balanceService;
    private readonly TransactionsAPIProvider _transactionsApiProvider;

    public TransactionsAPIService(ApplicationDbContext applicationDbContext, BalanceService balanceService,
        TransactionsAPIProvider transactionsApiProvider)
    {
        _applicationDbContext = applicationDbContext;
        _balanceService = balanceService;
        _transactionsApiProvider = transactionsApiProvider;
    }

    public async Task DeleteTransactionById(int id)
    {
        var transactionEntity = await _transactionsApiProvider.FindTransactionById(id);
        if (transactionEntity != null)
        {
            await _balanceService.ClearFromBalance(transactionEntity);
            _applicationDbContext.Transactions.Remove(transactionEntity);
        }

        await _applicationDbContext.SaveChangesAsync();
    }


    public async Task CreateTransaction(TransactionDTO transactionDTO, string userId)
    {
        _applicationDbContext.Transactions.Add(TransactionMapper.MapEntity(transactionDTO, userId));
        await _applicationDbContext.SaveChangesAsync();
    }

    public async Task EditTransaction(TransactionDTO transactionDTO)
    {
        var transaction = await _transactionsApiProvider.FindTransactionById(transactionDTO.Id);
        if (transaction != null)
        {
            transaction.Category = Enum.Parse<Category>(transactionDTO.Category);
            transaction.Date = transactionDTO.Date;
            transaction.Currency = Enum.Parse<Currency>(transactionDTO.Currency);
            transaction.Value = transactionDTO.Value;
            transaction.Location = transactionDTO.Location;
            transaction.Name = transactionDTO.Name;
            transaction.Note = transactionDTO.Note;
            _applicationDbContext.Entry(transaction).State = EntityState.Modified;
        }

        await _applicationDbContext.SaveChangesAsync();
    }
}