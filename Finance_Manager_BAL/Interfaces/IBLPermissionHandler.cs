using System.Collections.Generic;
using System.Threading.Tasks;
using Finance_Manager_MAL.DTO;

namespace Finance_Manager_BAL.Interfaces
{
    public interface IBLPermissionHandler
    {
        Task<ApiResponse<PagedData<DTOPermission>>> GetAllPermissionsAsync(PaginationQuery query);
        Task<ApiResponse<DTOPermission>> GetPermissionByIdAsync(long id);
        Task<ApiResponse<Dictionary<string, List<DTOPermission>>>> GetPermissionsByModuleAsync();
    }
}
