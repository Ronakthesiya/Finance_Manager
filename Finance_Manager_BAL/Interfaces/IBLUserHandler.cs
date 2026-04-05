using System.Threading.Tasks;
using Finance_Manager_MAL.DTO;

namespace Finance_Manager_BAL.Interfaces
{
    public interface IBLUserHandler
    {
        Task<ApiResponse<PagedData<DTOUser>>> GetAllUsersAsync(PaginationQuery query);
        Task<ApiResponse<DTOUser>> GetUserByIdAsync(long id);
        Task<ApiResponse<DTOUser>> CreateUserAsync(DTOUserCreate request);
        Task<ApiResponse<string>> UpdateUserAsync(long id, DTOUserUpdate request);
        Task<ApiResponse<string>> DeleteUserAsync(long id);
    }
}
