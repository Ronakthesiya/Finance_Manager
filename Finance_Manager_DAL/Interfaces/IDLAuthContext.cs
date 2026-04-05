using System.Collections.Generic;
using System.Threading.Tasks;
using Finance_Manager_MAL.POCO;

namespace Finance_Manager_DAL.Interfaces
{
    public interface IDLAuthContext
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(long id);
        Task<List<string>> GetUserRolesAsync(long userId);
        Task<List<string>> GetUserPermissionsAsync(long userId);
    }
}
