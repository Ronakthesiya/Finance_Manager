using System.Collections.Generic;
using System.Threading.Tasks;
using Finance_Manager_DAL.Models;
using Finance_Manager_MAL.DTO;
using Finance_Manager_MAL.POCO;

namespace Finance_Manager_DAL.Interfaces
{
    public interface IDLRoleContext
    {
        Task<(List<RoleWithPermissions> Items, int TotalCount)> GetAllRolesAsync(PaginationQuery query);
        Task<RoleWithPermissions?> GetRoleByIdAsync(long id);
        Task<long> InsertRoleAsync(Role role);
        Task AssignPermissionsAsync(long roleId, List<long> permissionIds);
        Task UpdateRolePermissionsAsync(long roleId, List<long> permissionIds);
        Task DeleteRoleAsync(long id);
        Task UpdateRoleAsync(long id, DTORoleUpdate request);
        Task<bool> IsSlugExistsAsync(string slug, long id);
    }
}
