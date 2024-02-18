using expense_tracker.web.Data.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace expense_tracker.web.Data;

public class ApplicationDbContext : IdentityDbContext<CustomUserEntity>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<TransactionEntity> Transactions { get; set; }
    public DbSet<BalanceEntity> Balances { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<CustomUserEntity>()
            .HasMany<TransactionEntity>(n => n.Transactions)
            .WithOne(n => n.User)
            .HasForeignKey(n => n.UserId);

        builder.Entity<CustomUserEntity>()
            .HasMany<BalanceEntity>(n => n.Balances)
            .WithOne(n => n.User)
            .HasForeignKey(n => n.UserId);
    }
}