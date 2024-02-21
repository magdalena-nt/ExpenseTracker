using System.Security.Claims;
using expense_tracker.web.Models.DTOs;
using expense_tracker.web.Services.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace expense_tracker.web.Controllers.API
{
    [Authorize(AuthenticationSchemes = "BasicAuthentication")]
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsWriteController : ControllerBase
    {
        private readonly TransactionsAPIService _transactionsApiService;

        public TransactionsWriteController(TransactionsAPIService transactionsApiService)
        {
            _transactionsApiService = transactionsApiService;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransactionDTO(int id, TransactionDTO transactionDTO)
        {
            if (id != transactionDTO.Id)
            {
                return BadRequest();
            }

            await _transactionsApiService.EditTransaction(transactionDTO);

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult<TransactionDTO>> PostTransactionDTO(TransactionDTO transactionDTO)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            await _transactionsApiService.CreateTransaction(transactionDTO, userId!);
            return CreatedAtAction("GetTransactionDTO", "TransactionsGet", new { id = transactionDTO.Id },
                transactionDTO);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransactionDTO(int id)
        {
            await _transactionsApiService.DeleteTransactionById(id);

            return NoContent();
        }
    }
}