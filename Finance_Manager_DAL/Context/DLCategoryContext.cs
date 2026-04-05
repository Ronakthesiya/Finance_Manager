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
    /// Handles database operations for categories
    /// </summary>
    public class DLCategoryContext : IDLCategoryContext
    {
        private readonly IDbConnectionFactory _dbFactory;

        public DLCategoryContext(IDbConnectionFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        /// <summary>
        /// Retrieves a paginated list of all active categories.
        /// </summary>
        public async Task<(List<Category> Items, int TotalCount)> GetAllAsync(PaginationQuery query)
        {
            using var db = _dbFactory.Open();

            var totalCount = await db.ScalarAsync<int>("SELECT COUNT(*) FROM category WHERE isactive = 1");

            var items = await db.SelectAsync<Category>(
                "SELECT * FROM category WHERE isactive = 1 LIMIT @Limit OFFSET @Offset",
                new { Limit = query.PageSize, Offset = (query.PageNumber - 1) * query.PageSize }
            );

            return (items, totalCount);
        }

        /// <summary>
        /// Retrieves a specific category record by its unique identifier.
        /// </summary>
        public async Task<Category?> GetByIdAsync(long id)
        {
            using var db = _dbFactory.Open();

            return await db.SingleAsync<Category>(
                "SELECT * FROM category WHERE id = @id",
                new { id });
        }

        /// <summary>
        /// Inserts a new category into the database and returns its assigned ID.
        /// </summary>
        public async Task<long> InsertAsync(Category category)
        {
            using var db = _dbFactory.Open();

            return await db.InsertAsync(category, selectIdentity: true);
        }

        /// <summary>
        /// Updates the core details and active status of an existing category.
        /// </summary>
        public async Task UpdateAsync(long id, DTOCategoryUpdate request)
        {
            using var db = _dbFactory.Open();

            await db.ExecuteSqlAsync(@"
            UPDATE category
            SET name = @name,
                type = @type,
                color = @color,
                isactive = @isActive
            WHERE id = @id",
                new
                {
                    id,
                    name = request.Name,
                    type = request.Type,
                    color = request.Color,
                    isActive = request.IsActive
                });
        }

        /// <summary>
        /// Performs a soft delete on a category by marking its active status to false.
        /// </summary>
        public async Task DeleteAsync(long id)
        {
            using var db = _dbFactory.Open();

            await db.ExecuteSqlAsync(
                "UPDATE category SET isactive = 0 WHERE id = @id",
                new { id });
        }
    }
}
