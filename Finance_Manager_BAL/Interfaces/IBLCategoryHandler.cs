using System.Threading.Tasks;
using Finance_Manager_MAL.DTO;

namespace Finance_Manager_BAL.Interfaces
{
    public interface IBLCategoryHandler
    {
        Task<ApiResponse<PagedData<DTOCategory>>> GetAllAsync(PaginationQuery query);
        Task<ApiResponse<DTOCategory>> GetByIdAsync(long id);
        Task<ApiResponse<DTOCategory>> CreateAsync(DTOCategoryCreate request);
        Task<ApiResponse<string>> UpdateAsync(long id, DTOCategoryUpdate request);
        Task<ApiResponse<string>> DeleteAsync(long id);
    }
}
