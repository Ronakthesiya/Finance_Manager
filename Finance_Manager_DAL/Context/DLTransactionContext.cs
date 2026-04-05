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
    /// Handles database operations for transactions
    /// </summary>
    public class DLTransactionContext : IDLTransactionContext
    {
        private readonly IDbConnectionFactory _dbFactory;

        public DLTransactionContext(IDbConnectionFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        /// <summary>
        /// Retrieves a paginated and filtered list of transactions from the database.
        /// </summary>
        public async Task<(List<Transaction> Items, int TotalCount)> GetAllAsync(TransactionFilterQuery query)
        {
            using var db = _dbFactory.Open();

            var q = db.From<Transaction>().Where(t => t.IsDeleted == false);

            if (query.StartDate.HasValue)
                q = q.Where(t => t.TransactionDate >= query.StartDate.Value);

            if (query.EndDate.HasValue)
                q = q.Where(t => t.TransactionDate <= query.EndDate.Value);

            if (query.CategoryId.HasValue)
                q = q.Where(t => t.CategoryId == query.CategoryId.Value);

            if (query.Type.HasValue)
                q = q.Where(t => t.Type == query.Type.Value);

            if (query.Status.HasValue)
                q = q.Where(t => t.Status == query.Status.Value);

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var lowerSearch = query.SearchTerm.ToLower();
                q = q.Where(t => t.Title.ToLower().Contains(lowerSearch) || (t.Note != null && t.Note.ToLower().Contains(lowerSearch)));
            }

            var totalCount = (int)await db.CountAsync(q);

            q = q.OrderByDescending(t => t.TransactionDate)
                 .Skip((query.PageNumber - 1) * query.PageSize)
                 .Take(query.PageSize);

            var items = await db.SelectAsync(q);

            return (items, totalCount);
        }

        /// <summary>
        /// Retrieves a specific transaction record by its unique identifier.
        /// </summary>
        public async Task<Transaction?> GetByIdAsync(long id)
        {
            using var db = _dbFactory.Open();

            return await db.SingleAsync<Transaction>(
                "SELECT * FROM transaction WHERE id = @id AND isdeleted = 0",
                new { id });
        }

        /// <summary>
        /// Inserts a new transaction record into the database and returns its assigned ID.
        /// </summary>
        public async Task<long> InsertAsync(Transaction txn)
        {
            using var db = _dbFactory.Open();

            return await db.InsertAsync(txn, selectIdentity: true);
        }

        /// <summary>
        /// Updates the core details of an existing transaction record.
        /// </summary>
        public async Task UpdateAsync(long id, DTOTransactionUpdate request)
        {
            using var db = _dbFactory.Open();

            await db.ExecuteSqlAsync(@"
            UPDATE transaction
            SET title = @title,
                amount = @amount,
                type = @type,
                transactiondate = @date,
                note = @note,
                categoryid = @categoryId
            WHERE id = @id",
                new
                {
                    id,
                    title = request.Title,
                    amount = request.Amount,
                    type = request.Type,
                    date = request.TransactionDate,
                    note = request.Note,
                    categoryId = request.CategoryId
                });
        }

        /// <summary>
        /// Modifies the approval status of a specific transaction.
        /// </summary>
        public async Task UpdateStatusAsync(long id, string status)
        {
            using var db = _dbFactory.Open();

            await db.ExecuteSqlAsync(@"
            UPDATE transaction
            SET status = @status
            WHERE id = @id",
                new { id, status });
        }

        /// <summary>
        /// Performs a soft delete on a transaction record by setting its IsDeleted flag.
        /// </summary>
        public async Task DeleteAsync(long id)
        {
            using var db = _dbFactory.Open();

            await db.ExecuteSqlAsync(
                "UPDATE transaction SET isdeleted = 1 WHERE id = @id",
                new { id });
        }
    }
}
