using Asp.Versioning;
using Finance_Manager_API.Filters;
using Finance_Manager_BAL.Interfaces;
using Finance_Manager_MAL.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance_Manager_API.Controllers
{
    /// <summary>
    /// Handles dashboard analytics APIs
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    [Permission("dashboard:view")]
    [Route("api/v{version:apiVersion}/dashboard")]
    public class CLDashboardController : ControllerBase
    {
        private readonly IBLDashboardHandler _handler;

        public CLDashboardController(IBLDashboardHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Get overall summary (income, expense, balance)
        /// </summary>
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var result = await _handler.GetSummaryAsync();
            return Ok(result);
        }

        /// <summary>
        /// Get monthly income vs expense
        /// </summary>
        [HttpGet("monthly")]
        public async Task<IActionResult> GetMonthlyStats()
        {
            var result = await _handler.GetMonthlyStatsAsync();
            return Ok(result);
        }

        /// <summary>
        /// Get expense distribution by category
        /// </summary>
        [HttpGet("category-expense")]
        public async Task<IActionResult> GetCategoryStats()
        {
            var result = await _handler.GetCategoryStatsAsync();
            return Ok(result);
        }

        /// <summary>
        /// Get recent transactions
        /// </summary>
        [HttpGet("recent-transactions")]
        public async Task<IActionResult> GetRecentTransactions([FromQuery] DashboardFilterQuery query)
        {
            var result = await _handler.GetRecentTransactionsAsync(query);
            return Ok(result);
        }

        /// <summary>
        /// Get daily stats
        /// </summary>
        [HttpGet("daily")]
        public async Task<IActionResult> GetDailyStats([FromQuery] DashboardFilterQuery query)
        {
            var result = await _handler.GetDailyStatsAsync(query);
            return Ok(result);
        }

        /// <summary>
        /// Get top expenses
        /// </summary>
        [HttpGet("top-expenses")]
        public async Task<IActionResult> GetTopExpenses([FromQuery] DashboardFilterQuery query)
        {
            var result = await _handler.GetTopExpensesAsync(query);
            return Ok(result);
        }

        /// <summary>
        /// Get pending transactions summary
        /// </summary>
        [HttpGet("pending-summary")]
        public async Task<IActionResult> GetPendingSummary()
        {
            var result = await _handler.GetPendingSummaryAsync();
            return Ok(result);
        }
    }
}
