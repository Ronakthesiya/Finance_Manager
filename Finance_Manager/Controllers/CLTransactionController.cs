using Asp.Versioning;
using Finance_Manager_API.Filters;
using Finance_Manager_BAL.Interfaces;
using Finance_Manager_MAL.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance_Manager_API.Controllers
{
    /// <summary>
    /// Handles transaction APIs
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    [Route("api/v{version:apiVersion}/transactions")]
    public class CLTransactionController : ControllerBase
    {
        private readonly IBLTransactionHandler _handler;

        public CLTransactionController(IBLTransactionHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Get all transactions
        /// </summary>
        [HttpGet]
        [Permission("transaction:view")]
        public async Task<IActionResult> GetAll([FromQuery] TransactionFilterQuery query)
        {
            var result = await _handler.GetAllAsync(query);
            return Ok(result);
        }

        /// <summary>
        /// Get transaction by ID
        /// </summary>
        [HttpGet("{id}")]
        [Permission("transaction:view")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _handler.GetByIdAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        /// <summary>
        /// Create transaction
        /// </summary>
        [HttpPost]
        [Permission("transaction:create")]
        public async Task<IActionResult> Create(DTOTransactionCreate request)
        {
            var result = await _handler.CreateAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Update transaction
        /// </summary>
        [HttpPut("{id}")]
        [Permission("transaction:update")]
        public async Task<IActionResult> Update(long id, DTOTransactionUpdate request)
        {
            var result = await _handler.UpdateAsync(id, request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Approve / Reject transaction
        /// </summary>
        [HttpPatch("{id}/status")]
        [Permission("transaction:approve")]
        public async Task<IActionResult> UpdateStatus(long id, DTOTransactionStatusUpdate request)
        {
            var result = await _handler.UpdateStatusAsync(id, request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Delete transaction (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        [Permission("transaction:delete")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _handler.DeleteAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }
    }
}
