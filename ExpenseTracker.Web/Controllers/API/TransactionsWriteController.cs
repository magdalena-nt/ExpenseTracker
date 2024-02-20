using System.Security.Claims;
using expense_tracker.web.Data.Entity;
using expense_tracker.web.Models.DTOs;
using expense_tracker.web.Services.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace expense_tracker.web.Controllers.API
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsWriteController : ControllerBase
    {
        private readonly TransactionsService _transactionsService;
        private readonly UserManager<CustomUserEntity> _userManager;

        public TransactionsWriteController(TransactionsService transactionsService,
            UserManager<CustomUserEntity> userManager)
        {
            _transactionsService = transactionsService;
            _userManager = userManager;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransactionDTO(int id, TransactionDTO transactionDTO)
        {
            if (id != transactionDTO.Id)
            {
                return BadRequest();
            }

            await _transactionsService.EditTransaction(transactionDTO);

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult<TransactionDTO>> PostTransactionDTO(TransactionDTO transactionDTO)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            await _transactionsService.CreateTransaction(transactionDTO, userId!);
            return CreatedAtAction("GetTransactionDTO", "TransactionsGet", new { id = transactionDTO.Id },
                transactionDTO);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransactionDTO(int id)
        {
            await _transactionsService.DeleteTransactionById(id);

            return NoContent();
        }
    }
}