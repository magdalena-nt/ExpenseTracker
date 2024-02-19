using expense_tracker.web.Data.Entity;
using expense_tracker.web.Models;
using expense_tracker.web.Models.DTOs;

namespace expense_tracker.web.Services.Mappers;

public static class TransactionMapper
{
    public static TransactionViewModel MapVM(TransactionEntity transactionEntity)
    {
        return new TransactionViewModel
        {
            Value = transactionEntity.Value,
            Currency = transactionEntity.Currency,
            Category = transactionEntity.Category,
            Date = transactionEntity.Date,
            Location = transactionEntity.Location,
            Name = transactionEntity.Name,
            Note = transactionEntity.Note,
            Id = transactionEntity.Id
        };
    }

    public static TransactionDTO MapDTO(TransactionEntity transactionEntity)
    {
        return new TransactionDTO
        {
            Value = transactionEntity.Value,
            Currency = transactionEntity.Currency,
            Category = transactionEntity.Category,
            Date = transactionEntity.Date,
            Location = transactionEntity.Location,
            Name = transactionEntity.Name,
            Note = transactionEntity.Note,
            Id = transactionEntity.Id
        };
    }
}