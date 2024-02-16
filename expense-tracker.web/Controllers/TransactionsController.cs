using System.Text;
using expense_tracker.web.Data;
using expense_tracker.web.Data.Entity;
using expense_tracker.web.Models;
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

        public TransactionsController(ApplicationDbContext context, UserManager<CustomUserEntity> userManager)
        {
            _context = context;
            _userManager = userManager;
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
                await _context.Transactions.Where(t => !t.IsIncome && t.UserId.Equals(_userManager.GetUserId(User)))
                    .OrderByDescending(t => t.Date).ToListAsync());
        }

        public async Task<IActionResult> Incomes()
        {
            return View("Index",
                await _context.Transactions.Where(t => t.IsIncome && t.UserId.Equals(_userManager.GetUserId(User)))
                    .OrderByDescending(t => t.Date).ToListAsync());
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
                    var incomes = group.Where(t => t.IsIncome).Sum(t => t.Value);
                    var expenses = group.Where(t => !t.IsIncome).Sum(t => t.Value);
                    return new BalanceByCurrency
                    {
                        Currency = group.Key,
                        TotalIncome = incomes,
                        TotalExpenses = expenses,
                        Balance = incomes - expenses
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
            var transactions = await GetTransactions();
            string json = JsonConvert.SerializeObject(transactions, Formatting.Indented);

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

            var transactionEntity = await _context.Transactions
                .FirstOrDefaultAsync(m => m.Id == id);
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,Value,Currency,Name,Note,Date,Location,IsIncome")]
            TransactionEntity transactionEntity)
        {
            ModelState.Remove("UserId");
            ModelState.Remove("UserEntity");

            if (ModelState.IsValid)
            {
                transactionEntity.UserId = _userManager.GetUserId(User)!;
                transactionEntity.UserEntity = (await _userManager.GetUserAsync(User))!;
                _context.Add(transactionEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(transactionEntity);
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,Value,Currency,Name,Note,Date,Location,IsIncome")]
            TransactionEntity transactionEntity)
        {
            if (id != transactionEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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

        public async Task<IList<TransactionEntity>> GetTransactions()
        {
            return await _context.Transactions.Where(t => t.UserId.Equals(_userManager.GetUserId(User)))
                .OrderByDescending(t => t.Date).ToListAsync();
        }
    }
}