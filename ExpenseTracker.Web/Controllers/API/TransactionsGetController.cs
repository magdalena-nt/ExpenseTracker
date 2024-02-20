using expense_tracker.web.Data;
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
        private readonly ApplicationDbContext _context;
        private readonly TransactionsProvider _transactionsProvider;
        private readonly BalanceService _balanceService;

        public TransactionsGetController(ApplicationDbContext context, BalanceService balanceService,
            TransactionsProvider transactionsProvider)
        {
            _context = context;
            _balanceService = balanceService;
            _transactionsProvider = transactionsProvider;
        }

        // GET: api/Get
        [HttpGet]
        public async Task<IEnumerable<TransactionDTO>> GetTransactions() =>
            await _transactionsProvider.FindAllTransactionDTOs();


        // GET: api/Get/expenses
        [HttpGet("expenses")]
        public async Task<IEnumerable<TransactionDTO>> GetExpenses() => await _transactionsProvider.FindExpensesDTOs();


        // GET: api/Get/expenses/2024/2
        [HttpGet("expenses/{year}/{month}")]
        public async Task<IEnumerable<TransactionDTO>> GetExpensesByYearMonth(int year, int month) =>
            await _transactionsProvider.FindExpensesDTOsByYearMonth(year, month);


        // GET: api/Get/income
        [HttpGet("income")]
        public async Task<IEnumerable<TransactionDTO>> GetIncome() =>
            await _transactionsProvider.FindIncomeDTOs();


        // GET: api/Get/income/2024/2
        [HttpGet("income/{year}/{month}")]
        public async Task<IEnumerable<TransactionDTO>> GetIncomeByYearMonth(int year, int month) =>
            await _transactionsProvider.FindIncomeDTOsByYearMonth(year, month);


        // GET: api/Get/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionDTO>> GetTransactionDTO(int id)
        {
            var transactionDTO = await _context.TransactionDTO.FindAsync(id);

            if (transactionDTO == null)
            {
                return NotFound();
            }

            return transactionDTO;
        }

        [HttpGet("categories")]
        public async Task<Dictionary<string, decimal>> GetTransactionSumGroupedByCategory() =>
            await _transactionsProvider.FindTransactionSumGroupedByCategory();


        [HttpGet("balance/{year}/{month}")]
        public async Task<Dictionary<Currency, decimal>>
            GetBalanceSumByYearMonthGroupedByCurrency(int year, int month) =>
            await _balanceService.FindBalanceSumByYearMonth(year, month);


        [HttpGet("balance/{year}/{month}/{currency}")]
        public async Task<BalanceSumByYearMonthCurrencyDTO> GetBalanceSumByYearMonthCurrency(int year,
            int month, string currency) =>
            await _balanceService.FindBalanceSumByYearMonthCurrency(year, month, currency);
    }
}