using System.Collections.Generic;
using System.Threading.Tasks;
using Finance_Manager_MAL.DTO;

namespace Finance_Manager_BAL.Interfaces
{
    public interface IBLDashboardHandler
    {
        Task<ApiResponse<DTODashboardSummary>> GetSummaryAsync();
        Task<ApiResponse<List<DTOMonthlyStats>>> GetMonthlyStatsAsync();
        Task<ApiResponse<List<DTOCategoryStats>>> GetCategoryStatsAsync();
        Task<ApiResponse<List<DTOTransaction>>> GetRecentTransactionsAsync(DashboardFilterQuery query);
        Task<ApiResponse<List<DTODailyStats>>> GetDailyStatsAsync(DashboardFilterQuery query);
        Task<ApiResponse<List<DTOTransaction>>> GetTopExpensesAsync(DashboardFilterQuery query);
        Task<ApiResponse<DTOPendingSummary>> GetPendingSummaryAsync();
    }
}
