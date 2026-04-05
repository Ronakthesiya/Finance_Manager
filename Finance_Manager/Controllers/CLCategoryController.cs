using Asp.Versioning;
using Finance_Manager_API.Filters;
using Finance_Manager_BAL.Interfaces;
using Finance_Manager_MAL.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance_Manager_API.Controllers
{

    /// <summary>
    /// Handles category APIs
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    [Route("api/v{version:apiVersion}/categories")]
    public class CLCategoryController : ControllerBase
    {
        private readonly IBLCategoryHandler _handler;

        public CLCategoryController(IBLCategoryHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Get all categories
        /// </summary>
        [HttpGet]
        [Permission("category:view")]
        public async Task<IActionResult> GetAll([FromQuery] PaginationQuery query)
        {
            var result = await _handler.GetAllAsync(query);
            return Ok(result);
        }

        /// <summary>
        /// Get category by ID
        /// </summary>
        [HttpGet("{id}")]
        [Permission("category:view")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _handler.GetByIdAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        /// <summary>
        /// Create category
        /// </summary>
        [HttpPost]
        [Permission("category:create")]
        public async Task<IActionResult> Create(DTOCategoryCreate request)
        {
            var result = await _handler.CreateAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Update category
        /// </summary>
        [HttpPut("{id}")]
        [Permission("category:update")]
        public async Task<IActionResult> Update(long id, DTOCategoryUpdate request)
        {
            var result = await _handler.UpdateAsync(id, request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Delete category (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        [Permission("category:delete")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _handler.DeleteAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }
    }
}
