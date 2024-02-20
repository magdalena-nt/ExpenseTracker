using expense_tracker.web.Data.Entity;
using expense_tracker.web.Models;
using expense_tracker.web.Models.DTOs;
using expense_tracker.web.Models.Enums;

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
            Currency = transactionEntity.Currency.ToString(),
            Category = transactionEntity.Category.ToString(),
            Date = transactionEntity.Date,
            Location = transactionEntity.Location,
            Name = transactionEntity.Name,
            Note = transactionEntity.Note,
            Id = transactionEntity.Id
        };
    }

    public static TransactionEntity MapEntity(TransactionDTO transactionDTO, string userId)
    {
        return new TransactionEntity
        {
            Value = transactionDTO.Value,
            Currency = Enum.Parse<Currency>(transactionDTO.Currency),
            Category = Enum.Parse<Category>(transactionDTO.Category),
            Date = transactionDTO.Date,
            Location = transactionDTO.Location,
            Name = transactionDTO.Name,
            Note = transactionDTO.Note,
            UserId = userId
        };
    }
}