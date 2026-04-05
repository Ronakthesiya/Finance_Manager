using System;
using Finance_Manager_DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finance_Manager_DAL.Models;
using Finance_Manager_MAL.DTO;
using Finance_Manager_MAL.POCO;
using ServiceStack.Data;
using ServiceStack.OrmLite;


namespace Finance_Manager_DAL.Context
{
    
    /// <summary>
    /// Handles database operations for roles
    /// </summary>
    public class DLRoleContext : IDLRoleContext
    {
        private readonly IDbConnectionFactory _dbFactory;

        public DLRoleContext(IDbConnectionFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        /// <summary>
        /// Retrieves a paginated list of active roles along with their associated permission slugs.
        /// </summary>
        public async Task<(List<RoleWithPermissions> Items, int TotalCount)> GetAllRolesAsync(PaginationQuery query)
        {
            using var db = _dbFactory.Open();

            var totalCount = await db.ScalarAsync<int>("SELECT COUNT(*) FROM role WHERE isdeleted = 0");

            var roles = await db.SelectAsync<Role>(
                "SELECT * FROM role WHERE isdeleted = 0 LIMIT @Limit OFFSET @Offset",
                new { Limit = query.PageSize, Offset = (query.PageNumber - 1) * query.PageSize }
            );

            var result = new List<RoleWithPermissions>();

            foreach (var role in roles)
            {
                var permissions = await db.ColumnAsync<string>(@"
                SELECT p.slug
                FROM rolepermission rp
                JOIN permission p ON rp.permissionid = p.id
                WHERE rp.roleid = @roleId
            ", new { roleId = role.Id });

                result.Add(new RoleWithPermissions
                {
                    Role = role,
                    Permissions = permissions.ToList()
                });
            }

            return (result, totalCount);
        }

        /// <summary>
        /// Retrieves a specific role record and its associated permissions by the role's unique ID.
        /// </summary>
        public async Task<RoleWithPermissions?> GetRoleByIdAsync(long id)
        {
            using var db = _dbFactory.Open();

            var role = await db.SingleAsync<Role>(
                "SELECT * FROM role WHERE id = @id AND isdeleted = 0",
                new { id });

            if (role == null) return null;

            var permissions = await db.ColumnAsync<string>(@"
            SELECT p.slug
            FROM rolepermission rp
            JOIN permission p ON rp.permissionid = p.id
            WHERE rp.roleid = @roleId
        ", new { roleId = id });

            return new RoleWithPermissions
            {
                Role = role,
                Permissions = permissions.ToList()
            };
        }

        /// <summary>
        /// Inserts a new role record into the database and returns the generated ID.
        /// </summary>
        public async Task<long> InsertRoleAsync(Role role)
        {
            using var db = _dbFactory.Open();

            return await db.InsertAsync(role, selectIdentity: true);
        }

        /// <summary>
        /// Associates a role with a list of permissions by inserting into the rolepermission mapping table.
        /// </summary>
        public async Task AssignPermissionsAsync(long roleId, List<long> permissionIds)
        {
            using var db = _dbFactory.Open();

            foreach (var pid in permissionIds)
            {
                await db.ExecuteSqlAsync(
                    "INSERT INTO rolepermission(roleid, permissionid) VALUES(@roleId, @pid)",
                    new { roleId, pid });
            }
        }

        /// <summary>
        /// Completely replaces a role's assigned permissions with a new set of permission IDs.
        /// </summary>
        public async Task UpdateRolePermissionsAsync(long roleId, List<long> permissionIds)
        {
            using var db = _dbFactory.Open();

            await db.ExecuteSqlAsync("DELETE FROM rolepermission WHERE roleid = @roleId", new { roleId });

            foreach (var pid in permissionIds)
            {
                await db.ExecuteSqlAsync(
                    "INSERT INTO rolepermission(roleid, permissionid) VALUES(@roleId, @pid)",
                    new { roleId, pid });
            }
        }

        /// <summary>
        /// Performs a soft delete on a role by setting its IsDeleted flag to true.
        /// </summary>
        public async Task DeleteRoleAsync(long id)
        {
            using var db = _dbFactory.Open();

            await db.ExecuteSqlAsync(
                "UPDATE role SET isdeleted = 1 WHERE id = @id",
                new { id });
        }

        /// <summary>
        /// Updates the core details (name, slug, and active status) of an existing role.
        /// </summary>
        public async Task UpdateRoleAsync(long id, DTORoleUpdate request)
        {
            using var db = _dbFactory.Open();

            await db.ExecuteSqlAsync(@"
        UPDATE role
        SET name = @name,
            slug = @slug,
            isactive = @isActive
        WHERE id = @id",
                new
                {
                    id,
                    name = request.Name,
                    slug = request.Slug,
                    isActive = request.IsActive
                });
        }

        /// <summary>
        /// Checks if a given slug already exists for any role other than the specified role ID.
        /// </summary>
        public async Task<bool> IsSlugExistsAsync(string slug, long id)
        {
            using var db = _dbFactory.Open();

            var count = await db.ScalarAsync<int>(@"
        SELECT COUNT(*)
        FROM role
        WHERE slug = @slug AND id != @id AND isdeleted = 0",
                new { slug, id });

            return count > 0;
        }
    }
}
