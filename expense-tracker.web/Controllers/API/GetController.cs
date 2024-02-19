using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using expense_tracker.web.Data;
using expense_tracker.web.Models.DTOs;
using expense_tracker.web.Models.Enums;
using expense_tracker.web.Services;

namespace expense_tracker.web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly TransactionService _transactionService;
        private readonly BalanceService _balanceService;

        public GetController(ApplicationDbContext context, TransactionService transactionService,
            BalanceService balanceService)
        {
            _context = context;
            _transactionService = transactionService;
            _balanceService = balanceService;
        }

        // GET: api/Get
        [HttpGet]
        public async Task<IEnumerable<TransactionDTO>> GetTransactions() =>
            await _transactionService.FindAllTransactionDTOs();


        // GET: api/Get/expenses
        [HttpGet("expenses")]
        public async Task<IEnumerable<TransactionDTO>> GetExpenses() => await _transactionService.FindExpensesDTOs();


        // GET: api/Get/expenses/2024/2
        [HttpGet("expenses/{year}/{month}")]
        public async Task<IEnumerable<TransactionDTO>> GetExpensesByYearMonth(int year, int month) =>
            await _transactionService.FindExpensesDTOsByYearMonth(year, month);


        // GET: api/Get/income
        [HttpGet("income")]
        public async Task<IEnumerable<TransactionDTO>> GetIncome() =>
            await _transactionService.FindIncomeDTOs();


        // GET: api/Get/income/2024/2
        [HttpGet("income/{year}/{month}")]
        public async Task<IEnumerable<TransactionDTO>> GetIncomeByYearMonth(int year, int month) =>
            await _transactionService.FindIncomeDTOsByYearMonth(year, month);


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
        public async Task<Dictionary<Category, decimal>> GetTransactionSumGroupedByCategory() =>
            await _transactionService.FindTransactionSumGroupedByCategory();


        [HttpGet("balance/{year}/{month}")]
        public async Task<Dictionary<Currency, decimal>>
            GetBalanceSumByYearMonthGroupedByCurrency(int year, int month) =>
            await _balanceService.FindBalanceSumByYearMonth(year, month);


        [HttpGet("balance/{year}/{month}/{currency}")]
        public async Task<IEnumerable<KeyValuePair<Currency, decimal>>> GetBalanceSumByYearMonthCurrency(int year,
            int month, string currency) =>
            await _balanceService.FindBalanceSumByYearMonthCurrency(year, month, currency);
    }
}