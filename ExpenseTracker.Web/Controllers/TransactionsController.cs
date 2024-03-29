using System.Text;
using expense_tracker.web.Data.Entity;
using expense_tracker.web.Models;
using expense_tracker.web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace expense_tracker.web.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private readonly UserManager<CustomUserEntity> _userManager;
        private readonly BalanceService _balanceService;
        private readonly TransactionsService _transactionsService;

        public TransactionsController(UserManager<CustomUserEntity> userManager,
            BalanceService balanceService, TransactionsService transactionsService)
        {
            _userManager = userManager;
            _balanceService = balanceService;
            _transactionsService = transactionsService;
        }

        // GET: Transactions
        public async Task<IActionResult> Index()
        {
            return View((await _transactionsService.FindTransactionVMsByUser(_userManager.GetUserId(User)))
                .OrderByDescending(t => t.Date).ToList());
        }

        public async Task<IActionResult> Expenses()
        {
            return View("Index", await _transactionsService.FindExpensesByUserAsync(_userManager.GetUserId(User)));
        }

        public async Task<IActionResult> Incomes()
        {
            return View("Index", await _transactionsService.FindIncomesByUserAsync(_userManager.GetUserId(User)));
        }

        public async Task<IActionResult> MonthlyBalance(int year, int month)
        {
            if (year == 0 || month == 0)
            {
                year = DateTime.UtcNow.Year;
                month = DateTime.UtcNow.Month;
            }

            var model = await _balanceService.FindMonthlyBalanceByDateAndUser(year, month,
                _userManager.GetUserId(User));


            return View(model);
        }

        public async Task<IActionResult> DownloadTransactions()
        {
            var findTransactionsByUser =
                (await _transactionsService.FindTransactionVMsByUser(_userManager.GetUserId(User)))
                .OrderByDescending(t => t.Date);

            var json = JsonConvert.SerializeObject(findTransactionsByUser, Formatting.Indented);

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

            var transactionEntity = await _transactionsService.FindTransactionVmById(id.Value);

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
                await _transactionsService.CreateTransaction(transactionVm, _userManager.GetUserId(User));
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

            var transactionViewModel = await _transactionsService.FindTransactionVmById(id.Value);
            if (transactionViewModel == null)
            {
                return NotFound();
            }

            return View(transactionViewModel);
        }

        // POST: Transactions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            TransactionViewModel transactionViewModel)
        {
            if (id != transactionViewModel.Id)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);

            if (ModelState.IsValid && userId != null)
            {
                await _transactionsService.EditTransactionByIdAsync(id, transactionViewModel);

                return RedirectToAction(nameof(Index));
            }

            return View(transactionViewModel);
        }

        // GET: Transactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transactionViewModel = await _transactionsService.FindTransactionVmById(id.Value);
            if (transactionViewModel == null)
            {
                return NotFound();
            }

            return View(transactionViewModel);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _transactionsService.DeleteTransactionById(id);
            return RedirectToAction(nameof(Index));
        }
    }
}