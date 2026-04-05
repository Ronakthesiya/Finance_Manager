using System.Collections.Generic;
using System.Threading.Tasks;
using Finance_Manager_DAL.Models;
using Finance_Manager_MAL.DTO;
using Finance_Manager_MAL.POCO;

namespace Finance_Manager_DAL.Interfaces
{
    public interface IDLUserContext
    {
        Task<(List<UserWithRoles> Items, int TotalCount)> GetAllUsersAsync(PaginationQuery query);
        Task<UserWithRoles?> GetUserByIdAsync(long id);
        Task<long> InsertUserAsync(User user);
        Task AssignRolesAsync(long userId, List<long> roleIds);
        Task UpdateUserAsync(long id, DTOUserUpdate request);
        Task UpdateUserRolesAsync(long userId, List<long> roleIds);
        Task DeleteUserAsync(long id);
    }
}
