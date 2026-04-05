using Finance_Manager_BAL.Interfaces;
using Finance_Manager_Core.Interface;
using Finance_Manager_DAL.Context;
using Finance_Manager_DAL.Interfaces;
using Finance_Manager_MAL.DTO;
using Finance_Manager_MAL.POCO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance_Manager_BAL
{
    /// <summary>
    /// Handles business logic for roles
    /// </summary>
    public class BLRoleHandler : IBLRoleHandler
    {
        private readonly IDLRoleContext _context;
        private readonly IDTOPOCOMapper _mapper;

        public BLRoleHandler(IDLRoleContext context, IDTOPOCOMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all roles along with their assigned permissions, sorted with pagination
        /// </summary>
        public async Task<ApiResponse<PagedData<DTORole>>> GetAllRolesAsync(PaginationQuery query)
        {
            var (roles, totalCount) = await _context.GetAllRolesAsync(query);

            var data = roles.Select(r =>
            {
                var dto = _mapper.Map<Role, DTORole>(r.Role);
                dto.Permissions = r.Permissions;
                return dto;
            }).ToList();

            var pagedData = new PagedData<DTORole>
            {
                Items = data,
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };

            return new ApiResponse<PagedData<DTORole>>
            {
                Success = true,
                Data = pagedData
            };
        }

        /// <summary>
        /// Retrieves a specific role and its assigned permissions by ID
        /// </summary>
        public async Task<ApiResponse<DTORole>> GetRoleByIdAsync(long id)
        {
            var role = await _context.GetRoleByIdAsync(id);

            if (role == null)
            {
                return new ApiResponse<DTORole>
                {
                    Success = false,
                    Message = "Role not found"
                };
            }

            var dto = _mapper.Map<Role, DTORole>(role.Role);
            dto.Permissions = role.Permissions;

            return new ApiResponse<DTORole>
            {
                Success = true,
                Data = dto
            };
        }

        /// <summary>
        /// Creates a new role and assigns default permissions
        /// </summary>
        public async Task<ApiResponse<DTORole>> CreateRoleAsync(DTORoleCreate request)
        {
            var role = _mapper.Map<DTORoleCreate, Role>(request);

            role.CreatedAt = DateTime.UtcNow;
            role.UpdatedAt = DateTime.UtcNow;
            role.IsActive = true;

            var roleId = await _context.InsertRoleAsync(role);

            await _context.AssignPermissionsAsync(roleId, request.PermissionIds);

            var created = await _context.GetRoleByIdAsync(roleId);

            var dto = _mapper.Map<Role, DTORole>(created.Role);
            dto.Permissions = created.Permissions;

            return new ApiResponse<DTORole>
            {
                Success = true,
                Message = "Role created successfully",
                Data = dto
            };
        }

        /// <summary>
        /// Soft deletes a role and marks it as inactive
        /// </summary>
        public async Task<ApiResponse<string>> DeleteRoleAsync(long id)
        {
            var exists = await _context.GetRoleByIdAsync(id);

            if (exists == null)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Role not found"
                };
            }

            await _context.DeleteRoleAsync(id);

            return new ApiResponse<string>
            {
                Success = true,
                Message = "Role deleted successfully"
            };
        }

        /// <summary>
        /// Updates the permission mappings for a specific role
        /// </summary>
        public async Task<ApiResponse<string>> UpdateRolePermissionsAsync(long roleId, List<long> permissionIds)
        {
            var exists = await _context.GetRoleByIdAsync(roleId);

            if (exists == null)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Role not found"
                };
            }

            await _context.UpdateRolePermissionsAsync(roleId, permissionIds);

            return new ApiResponse<string>
            {
                Success = true,
                Message = "Permissions updated successfully"
            };
        }

        /// <summary>
        /// Updates a role's core details and validates slug uniqueness
        /// </summary>
        public async Task<ApiResponse<string>> UpdateRoleAsync(long id, DTORoleUpdate request)
        {
            var exists = await _context.GetRoleByIdAsync(id);

            if (exists == null)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Role not found"
                };
            }

            // Optional: check duplicate slug
            var isDuplicate = await _context.IsSlugExistsAsync(request.Slug, id);

            if (isDuplicate)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Slug already exists"
                };
            }

            await _context.UpdateRoleAsync(id, request);

            return new ApiResponse<string>
            {
                Success = true,
                Message = "Role updated successfully"
            };
        }
    }
}
