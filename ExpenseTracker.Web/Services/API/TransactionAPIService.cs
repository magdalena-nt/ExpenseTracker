using expense_tracker.web.Data;
using expense_tracker.web.Data.Entity;
using expense_tracker.web.Models;
using expense_tracker.web.Models.DTOs;
using expense_tracker.web.Models.Enums;
using expense_tracker.web.Services.Mappers;
using Microsoft.EntityFrameworkCore;

namespace expense_tracker.web.Services.API;

public class TransactionAPIService
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly BalanceService _balanceService;

    public TransactionAPIService(ApplicationDbContext applicationDbContext, BalanceService balanceService)
    {
        _applicationDbContext = applicationDbContext;
        _balanceService = balanceService;
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

    public async Task<Dictionary<string, decimal>> FindTransactionSumGroupedByCategory()
    {
        var transactionDTOs = await FindAllTransactionDTOs();
        return transactionDTOs.GroupBy(t => t.Category)
            .ToDictionary(g => g.Key, g => g.Sum(dto => dto.Value));
    }

    public async Task CreateTransaction(TransactionDTO transactionDTO, string userId)
    {
        _applicationDbContext.Transactions.Add(TransactionMapper.MapEntity(transactionDTO, userId));
        await _applicationDbContext.SaveChangesAsync();
    }

    public async Task EditTransaction(TransactionDTO transactionDTO)
    {
        var transaction = await FindTransactionById(transactionDTO.Id);
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