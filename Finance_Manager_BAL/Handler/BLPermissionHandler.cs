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
    /// Handles business logic for permissions
    /// </summary>
    public class BLPermissionHandler : IBLPermissionHandler
    {
        private readonly IDLPermissionContext _context;
        private readonly IDTOPOCOMapper _mapper;

        public BLPermissionHandler(IDLPermissionContext context, IDTOPOCOMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all system permissions with pagination
        /// </summary>
        public async Task<ApiResponse<PagedData<DTOPermission>>> GetAllPermissionsAsync(PaginationQuery query)
        {
            var (permissions, totalCount) = await _context.GetAllPermissionsAsync(query);

            var data = permissions
                .Select(p => _mapper.Map<Permission, DTOPermission>(p))
                .ToList();

            var pagedData = new PagedData<DTOPermission>
            {
                Items = data,
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };

            return new ApiResponse<PagedData<DTOPermission>>
            {
                Success = true,
                Data = pagedData
            };
        }

        /// <summary>
        /// Retrieves a specific permission by its unique ID
        /// </summary>
        public async Task<ApiResponse<DTOPermission>> GetPermissionByIdAsync(long id)
        {
            var permission = await _context.GetPermissionByIdAsync(id);

            if (permission == null)
            {
                return new ApiResponse<DTOPermission>
                {
                    Success = false,
                    Message = "Permission not found"
                };
            }

            var dto = _mapper.Map<Permission, DTOPermission>(permission);

            return new ApiResponse<DTOPermission>
            {
                Success = true,
                Data = dto
            };
        }

        /// <summary>
        /// Retrieves all system permissions grouped by their module name
        /// </summary>
        public async Task<ApiResponse<Dictionary<string, List<DTOPermission>>>> GetPermissionsByModuleAsync()
        {
            // For groupby we get everything (we could paginate, but let's just get a large page for now)
            var (permissions, _) = await _context.GetAllPermissionsAsync(new PaginationQuery { PageSize = 10000 });

            var grouped = permissions
                .GroupBy(p => p.Module)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(p => _mapper.Map<Permission, DTOPermission>(p)).ToList()
                );

            return new ApiResponse<Dictionary<string, List<DTOPermission>>>
            {
                Success = true,
                Data = grouped
            };
        }
    }
}
