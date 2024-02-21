using expense_tracker.web.Models.DTOs;
using expense_tracker.web.Models.Enums;
using expense_tracker.web.Services;
using expense_tracker.web.Services.API;
using Microsoft.AspNetCore.Mvc;

namespace expense_tracker.web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsGetController : ControllerBase
    {
        private readonly TransactionsAPIProvider _transactionsApiProvider;
        private readonly BalanceService _balanceService;

        public TransactionsGetController(BalanceService balanceService,
            TransactionsAPIProvider transactionsApiProvider)
        {
            _balanceService = balanceService;
            _transactionsApiProvider = transactionsApiProvider;
        }

        // GET: api/Get
        [HttpGet]
        public async Task<IEnumerable<TransactionDTO>> GetTransactions() =>
            await _transactionsApiProvider.FindAllTransactionDTOs();


        // GET: api/Get/expenses
        [HttpGet("expenses")]
        public async Task<IEnumerable<TransactionDTO>> GetExpenses() =>
            await _transactionsApiProvider.FindExpensesDTOs();


        // GET: api/Get/expenses/2024/2
        [HttpGet("expenses/{year}/{month}")]
        public async Task<IEnumerable<TransactionDTO>> GetExpensesByYearMonth(int year, int month) =>
            await _transactionsApiProvider.FindExpensesDTOsByYearMonth(year, month);


        // GET: api/Get/income
        [HttpGet("income")]
        public async Task<IEnumerable<TransactionDTO>> GetIncome() =>
            await _transactionsApiProvider.FindIncomeDTOs();


        // GET: api/Get/income/2024/2
        [HttpGet("income/{year}/{month}")]
        public async Task<IEnumerable<TransactionDTO>> GetIncomeByYearMonth(int year, int month) =>
            await _transactionsApiProvider.FindIncomeDTOsByYearMonth(year, month);


        // GET: api/Get/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionDTO>> GetTransactionDTO(int id)
        {
            var transactionDTO = await _transactionsApiProvider.FindTransactionDTOById(id);
            if (transactionDTO == null)
            {
                return NotFound();
            }

            return transactionDTO;
        }

        [HttpGet("categories")]
        public async Task<Dictionary<string, decimal>> GetTransactionSumGroupedByCategory() =>
            await _transactionsApiProvider.FindTransactionSumGroupedByCategory();


        [HttpGet("balance/{year}/{month}")]
        public async Task<Dictionary<Currency, decimal>>
            GetBalanceSumByYearMonthGroupedByCurrency(int year, int month) =>
            await _balanceService.FindBalanceSumByYearMonth(year, month);


        [HttpGet("balance/{year}/{month}/{currency}")]
        public async Task<ActionResult<BalanceSumByYearMonthCurrencyDTO?>> GetBalanceSumByYearMonthCurrency(int year,
            int month, string currency)
        {
            var result = await _balanceService.FindBalanceSumByYearMonthCurrency(year, month, currency);
            if (result == null)
            {
                return NotFound();
            }

            return result;
        }
    }
}