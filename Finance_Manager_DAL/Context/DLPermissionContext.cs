using System;
using Finance_Manager_DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finance_Manager_MAL.POCO;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using Finance_Manager_MAL.DTO;

namespace Finance_Manager_DAL.Context
{
   
    /// <summary>
    /// Handles database operations for permissions
    /// </summary>
    public class DLPermissionContext : IDLPermissionContext
    {
        private readonly IDbConnectionFactory _dbFactory;

        public DLPermissionContext(IDbConnectionFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        /// <summary>
        /// Retrieves a paginated list of all permissions, sorted by module and name.
        /// </summary>
        public async Task<(List<Permission> Items, int TotalCount)> GetAllPermissionsAsync(PaginationQuery query)
        {
            using var db = _dbFactory.Open();

            var totalCount = await db.ScalarAsync<int>("SELECT COUNT(*) FROM permission");

            var items = await db.SelectAsync<Permission>(
                "SELECT * FROM permission ORDER BY module, name LIMIT @Limit OFFSET @Offset",
                new { Limit = query.PageSize, Offset = (query.PageNumber - 1) * query.PageSize }
            );

            return (items, totalCount);
        }

        /// <summary>
        /// Retrieves a specific permission record by its unique identifier.
        /// </summary>
        public async Task<Permission?> GetPermissionByIdAsync(long id)
        {
            using var db = _dbFactory.Open();

            return await db.SingleAsync<Permission>(
                "SELECT * FROM permission WHERE id = @id",
                new { id });
        }
    }
}
