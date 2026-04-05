using System.Collections.Generic;
using System.Threading.Tasks;
using Finance_Manager_MAL.DTO;
using Finance_Manager_MAL.POCO;

namespace Finance_Manager_DAL.Interfaces
{
    public interface IDLPermissionContext
    {
        Task<(List<Permission> Items, int TotalCount)> GetAllPermissionsAsync(PaginationQuery query);
        Task<Permission?> GetPermissionByIdAsync(long id);
    }
}
