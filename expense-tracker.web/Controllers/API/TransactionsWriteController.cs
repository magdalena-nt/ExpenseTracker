using System.Security.Claims;
using expense_tracker.web.Data.Entity;
using expense_tracker.web.Models.DTOs;
using expense_tracker.web.Services.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace expense_tracker.web.Controllers.API
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsWriteController : ControllerBase
    {
        private readonly TransactionAPIService _transactionService;
        private readonly UserManager<CustomUserEntity> _userManager;

        public TransactionsWriteController(TransactionAPIService transactionService,
            UserManager<CustomUserEntity> userManager)
        {
            _transactionService = transactionService;
            _userManager = userManager;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransactionDTO(int id, TransactionDTO transactionDTO)
        {
            if (id != transactionDTO.Id)
            {
                return BadRequest();
            }

            await _transactionService.EditTransaction(transactionDTO);

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult<TransactionDTO>> PostTransactionDTO(TransactionDTO transactionDTO)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            await _transactionService.CreateTransaction(transactionDTO, userId!);
            return CreatedAtAction("GetTransactionDTO", "TransactionsGet", new { id = transactionDTO.Id },
                transactionDTO);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransactionDTO(int id)
        {
            await _transactionService.DeleteTransactionById(id);

            return NoContent();
        }
    }
}