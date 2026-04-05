using Finance_Manager_BAL.Interfaces;
using Finance_Manager_Core.Interface;
using Finance_Manager_DAL.Context;
using Finance_Manager_DAL.Interfaces;
using Finance_Manager_MAL.DTO;
using Finance_Manager_MAL.POCO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance_Manager_BAL
{
    /// <summary>
    /// Handles business logic for dashboard
    /// </summary>
    public class BLDashboardHandler : IBLDashboardHandler
    {
        private readonly IDLDashboardContext _context;
        private readonly IDTOPOCOMapper _mapper;

        public BLDashboardHandler(IDLDashboardContext context, IDTOPOCOMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves overall dashboard summary including total income, expenses, and net balance
        /// </summary>
        public async Task<ApiResponse<DTODashboardSummary>> GetSummaryAsync()
        {
            var data = await _context.GetSummaryAsync();

            return new ApiResponse<DTODashboardSummary>
            {
                Success = true,
                Data = data
            };
        }

        /// <summary>
        /// Retrieves monthly aggregated income and expense statistics
        /// </summary>
        public async Task<ApiResponse<List<DTOMonthlyStats>>> GetMonthlyStatsAsync()
        {
            var data = await _context.GetMonthlyStatsAsync();

            return new ApiResponse<List<DTOMonthlyStats>>
            {
                Success = true,
                Data = data
            };
        }

        /// <summary>
        /// Retrieves expense distribution aggregated by category
        /// </summary>
        public async Task<ApiResponse<List<DTOCategoryStats>>> GetCategoryStatsAsync()
        {
            var data = await _context.GetCategoryStatsAsync();

            return new ApiResponse<List<DTOCategoryStats>>
            {
                Success = true,
                Data = data
            };
        }

        /// <summary>
        /// Retrieves the most recent approved transactions
        /// </summary>
        public async Task<ApiResponse<List<DTOTransaction>>> GetRecentTransactionsAsync(DashboardFilterQuery query)
        {
            var data = await _context.GetRecentTransactionsAsync(query);
            
            var mapped = data.Select(x => _mapper.Map<Transaction, DTOTransaction>(x)).ToList();
            
            return new ApiResponse<List<DTOTransaction>>
            {
                Success = true,
                Data = mapped
            };
        }

        /// <summary>
        /// Retrieves daily aggregated income and expense statistics over a specified date range
        /// </summary>
        public async Task<ApiResponse<List<DTODailyStats>>> GetDailyStatsAsync(DashboardFilterQuery query)
        {
            var data = await _context.GetDailyStatsAsync(query);

            return new ApiResponse<List<DTODailyStats>>
            {
                Success = true,
                Data = data
            };
        }

        /// <summary>
        /// Retrieves the highest single expense transactions
        /// </summary>
        public async Task<ApiResponse<List<DTOTransaction>>> GetTopExpensesAsync(DashboardFilterQuery query)
        {
            var data = await _context.GetTopExpensesAsync(query);
            
            var mapped = data.Select(x => _mapper.Map<Transaction, DTOTransaction>(x)).ToList();

            return new ApiResponse<List<DTOTransaction>>
            {
                Success = true,
                Data = mapped
            };
        }

        /// <summary>
        /// Retrieves the total count and amount of all pending transactions
        /// </summary>
        public async Task<ApiResponse<DTOPendingSummary>> GetPendingSummaryAsync()
        {
            var data = await _context.GetPendingSummaryAsync();

            return new ApiResponse<DTOPendingSummary>
            {
                Success = true,
                Data = data
            };
        }
    }
}
