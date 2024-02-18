using System.Text;
using expense_tracker.web.Data;
using expense_tracker.web.Data.Entity;
using expense_tracker.web.Models;
using expense_tracker.web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace expense_tracker.web.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<CustomUserEntity> _userManager;
        private readonly BalanceService _balanceService;
        private readonly TransactionService _transactionService;

        public TransactionsController(ApplicationDbContext context, UserManager<CustomUserEntity> userManager,
            BalanceService balanceService, TransactionService transactionService)
        {
            _context = context;
            _userManager = userManager;
            _balanceService = balanceService;
            _transactionService = transactionService;
        }

        // GET: Transactions
        public async Task<IActionResult> Index()
        {
            return View(await _context.Transactions.Where(t =>
                t.UserId.Equals(_userManager.GetUserId(User))).OrderByDescending(t => t.Date).ToListAsync());
        }

        public async Task<IActionResult> Expenses()
        {
            return View("Index",
                await _context.Transactions
                    .Where(t => t.Category < 0 && t.UserId.Equals(_userManager.GetUserId(User)))
                    .OrderByDescending(t => t.Date)
                    .ToListAsync());
        }

        public async Task<IActionResult> Incomes()
        {
            return View("Index",
                await _context.Transactions
                    .Where(t => t.Category > 0 && t.UserId.Equals(_userManager.GetUserId(User)))
                    .OrderByDescending(t => t.Date)
                    .ToListAsync());
        }

        public async Task<IActionResult> MonthlyBalance(int year, int month)
        {
            if (year == 0 || month == 0)
            {
                year = DateTime.UtcNow.Year;
                month = DateTime.UtcNow.Month;
            }

            var transactions = await _context.Transactions
                .Where(t => t.Date.Year == year && t.Date.Month == month)
                .ToListAsync();

            var balanceByCurrency = transactions
                .GroupBy(t => t.Currency)
                .Select(group =>
                {
                    // var incomes = group.Where(t => t.Category).Sum(t => t.Value);
                    // var expenses = group.Where(t => !t.Category).Sum(t => t.Value);
                    return new BalanceByCurrencyViewModel
                    {
                        // Currency = group.Key,
                        // TotalIncome = incomes,
                        // TotalExpenses = expenses,
                        // Balance = incomes - expenses
                    };
                })
                .ToList();

            var model = new MonthlyBalanceViewModel
            {
                Year = year,
                Month = month,
                BalancesByCurrency = balanceByCurrency
            };

            return View(model);
        }

        public async Task<IActionResult> DownloadTransactions()
        {
            var transactionEntities =
                _userManager.GetUserAsync(User).Result!.Transactions.OrderBy(t => t.Date);

            var json = JsonConvert.SerializeObject(transactionEntities, Formatting.Indented);

            var byteArray = Encoding.UTF8.GetBytes(json);

            return File(byteArray, "application/json", "Transactions.json");
        }

        // GET: Transactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transactionEntity = await _transactionService.FindTransactionById(id.Value);
            
            if (transactionEntity == null)
            {
                return NotFound();
            }

            return View(transactionEntity);
        }

        // GET: Transactions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Transactions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TransactionViewModel transactionVm)
        {
            if (ModelState.IsValid)
            {
                await _transactionService.CreateTransaction(transactionVm, _userManager.GetUserId(User));
                return RedirectToAction(nameof(Index));
            }

            return View(transactionVm);
        }

        // GET: Transactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transactionEntity = await _context.Transactions.FindAsync(id);
            if (transactionEntity == null)
            {
                return NotFound();
            }

            return View(transactionEntity);
        }

        // POST: Transactions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            TransactionEntity transactionEntity)
        {
            if (id != transactionEntity.Id)
            {
                return NotFound();
            }

            ModelState.Remove("UserId");
            ModelState.Remove("UserEntity");

            if (ModelState.IsValid)
            {
                try
                {
                    transactionEntity.UserId = _userManager.GetUserId(User)!;
                    _context.Update(transactionEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionEntityExists(transactionEntity.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(transactionEntity);
        }

        // GET: Transactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transactionEntity = await _context.Transactions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transactionEntity == null)
            {
                return NotFound();
            }

            return View(transactionEntity);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transactionEntity = await _context.Transactions.FindAsync(id);
            if (transactionEntity != null)
            {
                _context.Transactions.Remove(transactionEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransactionEntityExists(int id)
        {
            return _context.Transactions.Any(e => e.Id == id);
        }
    }
}