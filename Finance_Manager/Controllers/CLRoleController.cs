using Asp.Versioning;
using Finance_Manager_API.Filters;
using Finance_Manager_BAL.Interfaces;
using Finance_Manager_MAL.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance_Manager_API.Controllers
{

    /// <summary>
    /// Handles role management APIs
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    [Route("api/v{version:apiVersion}/roles")]
    public class CLRoleController : ControllerBase
    {
        private readonly IBLRoleHandler _handler;
        public CLRoleController(IBLRoleHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Get all roles
        /// </summary>
        [HttpGet]
        [Permission("role:view")]
        public async Task<IActionResult> GetAll([FromQuery] PaginationQuery query)
        {
            //throw new DivideByZeroException();
            var result = await _handler.GetAllRolesAsync(query);
            return Ok(result);
        }

        /// <summary>
        /// Get role by ID
        /// </summary>
        [HttpGet("{id}")]
        [Permission("role:view")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _handler.GetRoleByIdAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        /// <summary>
        /// Create new role
        /// </summary>
        [HttpPost]
        [Permission("role:create")]
        public async Task<IActionResult> Create(DTORoleCreate request)
        {
            var result = await _handler.CreateRoleAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Delete role (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        [Permission("role:delete")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _handler.DeleteRoleAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        /// <summary>
        /// Assign permissions to role
        /// </summary>
        [HttpPut("{id}/permissions")]
        [Permission("role:update")]
        public async Task<IActionResult> UpdatePermissions(long id, List<long> permissionIds)
        {
            var result = await _handler.UpdateRolePermissionsAsync(id, permissionIds);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Update role details
        /// </summary>
        [HttpPut("{id}")]
        [Permission("role:update")]
        public async Task<IActionResult> Update(long id, DTORoleUpdate request)
        {
            var result = await _handler.UpdateRoleAsync(id, request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
