using expense_tracker.web.Services;
using expense_tracker.web.Services.API;

namespace expense_tracker.web.Configuration;

public static class ServiceRegistration
{
    public static void RegisterDIServices(this IServiceCollection services)
    {
        services.AddTransient<BalanceService>();
        services.AddTransient<TransactionService>();
        services.AddTransient<TransactionAPIService>();
    }
}