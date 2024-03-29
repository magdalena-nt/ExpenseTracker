﻿using expense_tracker.web.Data;
using expense_tracker.web.Data.Entity;
using expense_tracker.web.Models;
using expense_tracker.web.Models.DTOs;
using expense_tracker.web.Models.Enums;
using expense_tracker.web.Services.Mappers;
using Microsoft.EntityFrameworkCore;

namespace expense_tracker.web.Services;

public class TransactionsService
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly BalanceService _balanceService;

    public TransactionsService(ApplicationDbContext applicationDbContext, BalanceService balanceService)
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

    public async Task<TransactionViewModel?> FindTransactionVmById(int id)
    {
        var transactionEntity = await FindTransactionById(id);
        if (transactionEntity != null)
        {
            return TransactionMapper.MapVM(transactionEntity);
        }

        return null;
    }


    public async Task<IEnumerable<TransactionViewModel>> FindTransactionVMsByUser(string? userId)
    {
        var transactionEntities =
            await _applicationDbContext.Transactions.Where(t => t.UserId.Equals(userId)).ToListAsync();
        var transactionVMs = transactionEntities.Select(TransactionMapper.MapVM).ToList();

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

    public async Task EditTransactionByIdAsync(int id, TransactionViewModel transactionViewModel)
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

    public async Task<IEnumerable<TransactionViewModel>> FindIncomesByUserAsync(string? userId)
    {
        var findTransactionVMsByUser = await FindTransactionVMsByUser(userId);
        return findTransactionVMsByUser.Where(t => t.Category > 0)
            .OrderByDescending(t => t.Date).ToList();
    }

    public async Task<IEnumerable<TransactionViewModel>> FindExpensesByUserAsync(string? userId)
    {
        var findTransactionVMsByUser = await FindTransactionVMsByUser(userId);
        return findTransactionVMsByUser.Where(t => t.Category < 0)
            .OrderByDescending(t => t.Date).ToList();
    }
}