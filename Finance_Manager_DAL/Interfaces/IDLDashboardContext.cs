using System.Collections.Generic;
using System.Threading.Tasks;
using Finance_Manager_MAL.DTO;
using Finance_Manager_MAL.POCO;

namespace Finance_Manager_DAL.Interfaces
{
    public interface IDLDashboardContext
    {
        Task<DTODashboardSummary> GetSummaryAsync();
        Task<List<DTOMonthlyStats>> GetMonthlyStatsAsync();
        Task<List<DTOCategoryStats>> GetCategoryStatsAsync();
        Task<List<Transaction>> GetRecentTransactionsAsync(DashboardFilterQuery query);
        Task<List<DTODailyStats>> GetDailyStatsAsync(DashboardFilterQuery query);
        Task<List<Transaction>> GetTopExpensesAsync(DashboardFilterQuery query);
        Task<DTOPendingSummary> GetPendingSummaryAsync();
    }
}
