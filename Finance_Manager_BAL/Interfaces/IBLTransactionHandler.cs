using System.Threading.Tasks;
using Finance_Manager_MAL.DTO;

namespace Finance_Manager_BAL.Interfaces
{
    public interface IBLTransactionHandler
    {
        Task<ApiResponse<PagedData<DTOTransaction>>> GetAllAsync(TransactionFilterQuery query);
        Task<ApiResponse<DTOTransaction>> GetByIdAsync(long id);
        Task<ApiResponse<DTOTransaction>> CreateAsync(DTOTransactionCreate request);
        Task<ApiResponse<string>> UpdateAsync(long id, DTOTransactionUpdate request);
        Task<ApiResponse<string>> UpdateStatusAsync(long id, DTOTransactionStatusUpdate request);
        Task<ApiResponse<string>> DeleteAsync(long id);
    }
}
