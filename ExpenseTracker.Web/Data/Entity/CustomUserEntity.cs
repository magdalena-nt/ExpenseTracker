using Microsoft.AspNetCore.Identity;

namespace expense_tracker.web.Data.Entity;

public class CustomUserEntity : IdentityUser
{
    public ICollection<TransactionEntity> Transactions { get; set; }
    public ICollection<BalanceEntity> Balances { get; set; }
}