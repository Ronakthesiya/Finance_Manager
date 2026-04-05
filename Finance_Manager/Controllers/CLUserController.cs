using Asp.Versioning;
using Finance_Manager_API.Filters;
using Finance_Manager_BAL.Interfaces;
using Finance_Manager_MAL.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance_Manager_API.Controllers
{
    /// <summary>
    /// Handles user management APIs
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    [Route("api/v{version:apiVersion}/users")]
    public class CLUserController : ControllerBase
    {
        private readonly IBLUserHandler _handler;

        public CLUserController(IBLUserHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Get all users
        /// </summary>
        [HttpGet]
        [Permission("user:view")]
        public async Task<IActionResult> GetAll([FromQuery] PaginationQuery query)
        {
            var result = await _handler.GetAllUsersAsync(query);
            return Ok(result);
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        [HttpGet("{id}")]
        [Permission("user:view")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _handler.GetUserByIdAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        /// <summary>
        /// Create new user
        /// </summary>
        [HttpPost]
        [Permission("user:create")]
        public async Task<IActionResult> Create(DTOUserCreate request)
        {
            var result = await _handler.CreateUserAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Update user
        /// </summary>
        [HttpPut("{id}")]
        [Permission("user:update")]
        public async Task<IActionResult> Update(long id, DTOUserUpdate request)
        {
            var result = await _handler.UpdateUserAsync(id, request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Delete user (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        [Permission("user:delete")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _handler.DeleteUserAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }
    }
}
