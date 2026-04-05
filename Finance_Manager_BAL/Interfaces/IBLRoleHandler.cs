using System.Collections.Generic;
using System.Threading.Tasks;
using Finance_Manager_MAL.DTO;

namespace Finance_Manager_BAL.Interfaces
{
    public interface IBLRoleHandler
    {
        Task<ApiResponse<PagedData<DTORole>>> GetAllRolesAsync(PaginationQuery query);
        Task<ApiResponse<DTORole>> GetRoleByIdAsync(long id);
        Task<ApiResponse<DTORole>> CreateRoleAsync(DTORoleCreate request);
        Task<ApiResponse<string>> DeleteRoleAsync(long id);
        Task<ApiResponse<string>> UpdateRolePermissionsAsync(long roleId, List<long> permissionIds);
        Task<ApiResponse<string>> UpdateRoleAsync(long id, DTORoleUpdate request);
    }
}
