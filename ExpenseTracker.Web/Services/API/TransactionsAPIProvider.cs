using expense_tracker.web.Data;
using expense_tracker.web.Data.Entity;
using expense_tracker.web.Models.DTOs;
using expense_tracker.web.Services.Mappers;
using Microsoft.EntityFrameworkCore;

namespace expense_tracker.web.Services.API;

public class TransactionsAPIProvider
{
    private readonly ApplicationDbContext _applicationDbContext;

    public TransactionsAPIProvider(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }


    public async Task<TransactionEntity?> FindTransactionById(int id)
    {
        var transactionEntity = await _applicationDbContext.Transactions
            .FirstOrDefaultAsync(m => m.Id == id);
        return transactionEntity;
    }


    public async Task<TransactionDTO?> FindTransactionDTOById(int id)
    {
        var transactionEntity = await FindTransactionById(id);
        if (transactionEntity != null)
        {
            return TransactionMapper.MapDTO(transactionEntity);
        }

        return null;
    }

    public async Task<IEnumerable<TransactionDTO>> FindAllTransactionDTOs()
    {
        return await _applicationDbContext.Transactions.Select(t => TransactionMapper.MapDTO(t)).ToListAsync();
    }

    public async Task<IEnumerable<TransactionDTO>> FindExpensesDTOs()
    {
        return await _applicationDbContext.Transactions
            .Where(t => t.Category < 0)
            .Select(t => TransactionMapper.MapDTO(t)).ToListAsync();
    }

    public async Task<IEnumerable<TransactionDTO>> FindIncomeDTOs()
    {
        return await _applicationDbContext.Transactions
            .Where(t => t.Category > 0)
            .Select(t => TransactionMapper.MapDTO(t)).ToListAsync();
    }

    public async Task<IEnumerable<TransactionDTO>> FindExpensesDTOsByYearMonth(int year, int month)
    {
        return await _applicationDbContext.Transactions
            .Where(t => t.Date.Year == year && t.Date.Month == month && t.Category < 0)
            .Select(t => TransactionMapper.MapDTO(t)).ToListAsync();
    }

    public async Task<IEnumerable<TransactionDTO>> FindIncomeDTOsByYearMonth(int year, int month)
    {
        return await _applicationDbContext.Transactions
            .Where(t => t.Date.Year == year && t.Date.Month == month && t.Category > 0)
            .Select(t => TransactionMapper.MapDTO(t)).ToListAsync();
    }


    public async Task<Dictionary<string, decimal>> FindTransactionSumGroupedByCategory()
    {
        var transactionDTOs = await FindAllTransactionDTOs();
        return transactionDTOs.GroupBy(t => t.Category)
            .ToDictionary(g => g.Key, g => g.Sum(dto => dto.Value));
    }
}