using System.Security.Claims;
using expense_tracker.web.Models.DTOs;
using expense_tracker.web.Services.API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace expense_tracker.web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsWriteController : ControllerBase
    {
        private readonly TransactionAPIService _transactionService;

        public TransactionsWriteController(TransactionAPIService transactionService)
        {
            _transactionService = transactionService;
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
            // var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var userId = "3a6c7d51-daab-4d89-92df-bf2af0e41d15";
            await _transactionService.CreateTransaction(transactionDTO, userId);

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