using expense_tracker.web.Data.Entity;
using expense_tracker.web.Models;

namespace expense_tracker.web.Services.Mappers;

public static class TransactionMapper
{
    public static TransactionEntity Map(TransactionViewModel transactionViewModel, string userId)
    {
        return new TransactionEntity
        {
            Currency = transactionViewModel.Currency,
            UserId = userId,
            Category = transactionViewModel.Category,
            Date = transactionViewModel.Date,
            Location = transactionViewModel.Location,
            Name = transactionViewModel.Name,
            Note = transactionViewModel.Note,
            Value = transactionViewModel.Value,
            Id = transactionViewModel.Id
        };
    }

    public static TransactionViewModel Map(TransactionEntity transactionEntity)
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
}