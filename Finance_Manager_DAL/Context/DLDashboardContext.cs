using System;
using Finance_Manager_DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finance_Manager_MAL.DTO;
using Finance_Manager_MAL.POCO;
using ServiceStack.Data;
using ServiceStack.OrmLite;

namespace Finance_Manager_DAL.Context
{
    
    /// <summary>
    /// Handles database queries for dashboard
    /// </summary>
    public class DLDashboardContext : IDLDashboardContext
    {
        private readonly IDbConnectionFactory _dbFactory;

        public DLDashboardContext(IDbConnectionFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        /// <summary>
        /// Retrieves the total income, total expense, and calculated net balance for all approved transactions.
        /// </summary>
        public async Task<DTODashboardSummary> GetSummaryAsync()
        {
            using var db = _dbFactory.Open();

            var income = await db.ScalarAsync<decimal>(@"
            SELECT IFNULL(SUM(amount),0)
            FROM transaction
            WHERE type = 'income' AND status = 'approved' AND isdeleted = 0");

            var expense = await db.ScalarAsync<decimal>(@"
            SELECT IFNULL(SUM(amount),0)
            FROM transaction
            WHERE type = 'expense' AND status = 'approved' AND isdeleted = 0");

            return new DTODashboardSummary
            {
                TotalIncome = income,
                TotalExpense = expense,
                NetBalance = income - expense
            };
        }

        /// <summary>
        /// Retrieves monthly aggregated income and expense statistics across all approved transactions.
        /// </summary>
        public async Task<List<DTOMonthlyStats>> GetMonthlyStatsAsync()
        {
            using var db = _dbFactory.Open();

            var sql = @"
            SELECT 
                DATE_FORMAT(transactiondate, '%Y-%m') AS Month,
                SUM(CASE WHEN type = 'income' THEN amount ELSE 0 END) AS Income,
                SUM(CASE WHEN type = 'expense' THEN amount ELSE 0 END) AS Expense
            FROM transaction
            WHERE status = 'approved' AND isdeleted = 0
            GROUP BY Month
            ORDER BY Month";

            return await db.SelectAsync<DTOMonthlyStats>(sql);
        }

        /// <summary>
        /// Retrieves total expenses grouped by category for all approved expense transactions.
        /// </summary>
        public async Task<List<DTOCategoryStats>> GetCategoryStatsAsync()
        {
            using var db = _dbFactory.Open();

            var sql = @"
            SELECT 
                c.name AS Category,
                SUM(t.amount) AS Total
            FROM transaction t
            JOIN category c ON t.categoryid = c.id
            WHERE t.type = 'expense' 
              AND t.status = 'approved'
              AND t.isdeleted = 0
            GROUP BY c.name
            ORDER BY Total DESC";

            return await db.SelectAsync<DTOCategoryStats>(sql);
        }

        /// <summary>
        /// Get recent transactions
        /// </summary>
        public async Task<List<Transaction>> GetRecentTransactionsAsync(DashboardFilterQuery query)
        {
            using var db = _dbFactory.Open();

            var q = db.From<Transaction>().Where(t => t.IsDeleted == false);
            if(query.StartDate.HasValue) q = q.Where(t => t.TransactionDate >= query.StartDate.Value);
            if(query.EndDate.HasValue) q = q.Where(t => t.TransactionDate <= query.EndDate.Value);

            q = q.OrderByDescending(t => t.TransactionDate).Limit(query.Limit);

            return await db.SelectAsync(q);
        }

        /// <summary>
        /// Get daily stats
        /// </summary>
        public async Task<List<DTODailyStats>> GetDailyStatsAsync(DashboardFilterQuery query)
        {
            using var db = _dbFactory.Open();
            
            var start = query.StartDate ?? DateTime.UtcNow.AddDays(-30);
            var end = query.EndDate ?? DateTime.UtcNow;

            var sql = @"
            SELECT 
                DATE_FORMAT(transactiondate, '%Y-%m-%d') AS Date,
                SUM(CASE WHEN type = 'income' THEN amount ELSE 0 END) AS Income,
                SUM(CASE WHEN type = 'expense' THEN amount ELSE 0 END) AS Expense
            FROM transaction
            WHERE status = 'approved' AND isdeleted = 0
              AND transactiondate >= @start AND transactiondate <= @end
            GROUP BY Date
            ORDER BY Date";

            return await db.SelectAsync<DTODailyStats>(sql, new { start, end });
        }

        /// <summary>
        /// Get top expenses
        /// </summary>
        public async Task<List<Transaction>> GetTopExpensesAsync(DashboardFilterQuery query)
        {
            using var db = _dbFactory.Open();

            var q = db.From<Transaction>()
                      .Where(t => t.Type == Finance_Manager_MAL.Enums.TransactionType.Expense && t.IsDeleted == false && t.Status == Finance_Manager_MAL.Enums.TransactionStatus.Approved);
            
            if(query.StartDate.HasValue) q = q.Where(t => t.TransactionDate >= query.StartDate.Value);
            if(query.EndDate.HasValue) q = q.Where(t => t.TransactionDate <= query.EndDate.Value);

            q = q.OrderByDescending(t => t.Amount).Limit(query.Limit);

            return await db.SelectAsync(q);
        }

        /// <summary>
        /// Get pending transactions summary
        /// </summary>
        public async Task<DTOPendingSummary> GetPendingSummaryAsync()
        {
            using var db = _dbFactory.Open();
            
            var amountSql = "SELECT IFNULL(SUM(amount), 0) FROM transaction WHERE status = 'pending' AND isdeleted = 0";
            var totalAmount = await db.ScalarAsync<decimal>(amountSql);

            var countSql = "SELECT COUNT(*) FROM transaction WHERE status = 'pending' AND isdeleted = 0";
            var count = await db.ScalarAsync<int>(countSql);

            return new DTOPendingSummary
            {
                PendingCount = count,
                TotalPendingAmount = totalAmount
            };
        }
    }
}
