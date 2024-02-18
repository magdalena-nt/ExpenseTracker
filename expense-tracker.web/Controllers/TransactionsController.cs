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
        private readonly UserManager<CustomUserEntity> _userManager;
        private readonly BalanceService _balanceService;
        private readonly TransactionService _transactionService;

        public TransactionsController(UserManager<CustomUserEntity> userManager,
            BalanceService balanceService, TransactionService transactionService)
        {
            _userManager = userManager;
            _balanceService = balanceService;
            _transactionService = transactionService;
        }

        // GET: Transactions
        public async Task<IActionResult> Index()
        {
            return View(_transactionService.FindTransactionVMsByUser(_userManager.GetUserId(User))
                .OrderByDescending(t => t.Date).ToList());
        }

        public async Task<IActionResult> Expenses()
        {
            return View("Index", _transactionService.FindExpensesByUser(_userManager.GetUserId(User)));
        }

        public async Task<IActionResult> Incomes()
        {
            return View("Index", _transactionService.FindIncomesByUser(_userManager.GetUserId(User)));
        }

        public async Task<IActionResult> MonthlyBalance(int year, int month)
        {
            if (year == 0 || month == 0)
            {
                year = DateTime.UtcNow.Year;
                month = DateTime.UtcNow.Month;
            }

            var model = _balanceService.FindMonthlyBalanceByDateAndUser(year, month, _userManager.GetUserId(User));


            return View(model);
        }

        public IActionResult DownloadTransactions()
        {
            var findTransactionsByUser = _transactionService.FindTransactionVMsByUser(_userManager.GetUserId(User))
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

            var transactionEntity = _transactionService.FindTransactionVmById(id.Value);

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

            var transactionViewModel = _transactionService.FindTransactionVmById(id.Value);
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
                try
                {
                    await _transactionService.EditTransactionById(id, userId, transactionViewModel);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (_transactionService.FindTransactionVmById(id) == null)
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(transactionViewModel);
        }

        // GET: Transactions/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transactionViewModel = _transactionService.FindTransactionVmById(id.Value);
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
            await _transactionService.DeleteTransactionById(id);
            return RedirectToAction(nameof(Index));
        }
    }
}