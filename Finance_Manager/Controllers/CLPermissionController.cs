using Asp.Versioning;
using Finance_Manager_API.Filters;
using Finance_Manager_BAL.Interfaces;
using Finance_Manager_MAL.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance_Manager_API.Controllers
{

    /// <summary>
    /// Handles permission APIs (read-only)
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    [Permission("permission:view")]
    [Route("api/v{version:apiVersion}/permissions")]
    public class CLPermissionController : ControllerBase
    {
        private readonly IBLPermissionHandler _handler;

        public CLPermissionController(IBLPermissionHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Get all permissions
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationQuery query)
        {

            var result = await _handler.GetAllPermissionsAsync(query);
            return Ok(result);
        }

        /// <summary>
        /// Get permission by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _handler.GetPermissionByIdAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        /// <summary>
        /// Get permissions grouped by module
        /// </summary>
        [HttpGet("by-module")]
        public async Task<IActionResult> GetByModule()
        {
            var result = await _handler.GetPermissionsByModuleAsync();
            return Ok(result);
        }
    }
}
