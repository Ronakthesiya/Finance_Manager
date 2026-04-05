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
    /// Handles database operations for users
    /// </summary>
    public class DLUserContext : IDLUserContext
    {   
        private readonly IDbConnectionFactory _dbFactory;

        public DLUserContext(IDbConnectionFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        /// <summary>
        /// Retrieves all active user records along with their assigned role names, sorted with pagination.
        /// </summary>
        public async Task<(List<UserWithRoles> Items, int TotalCount)> GetAllUsersAsync(PaginationQuery query)
        {
            using var db = _dbFactory.Open();

            var totalCount = await db.ScalarAsync<int>("SELECT COUNT(*) FROM user WHERE isdeleted = 0");

            var users = await db.SelectAsync<User>(
                "SELECT * FROM user WHERE isdeleted = 0 LIMIT @Limit OFFSET @Offset",
                new { Limit = query.PageSize, Offset = (query.PageNumber - 1) * query.PageSize }
            );

            var result = new List<UserWithRoles>();

            foreach (var user in users)
            {
                var roles = await db.ColumnAsync<string>(@"
                SELECT r.name
                FROM userrole ur
                JOIN role r ON ur.roleid = r.id
                WHERE ur.userid = @userId
            ", new { userId = user.Id });

                result.Add(new UserWithRoles
                {
                    User = user,
                    Roles = roles.ToList()
                });
            }

            return (result, totalCount);
        }

        /// <summary>
        /// Retrieves a specific user record and their assigned role names by the user's ID.
        /// </summary>
        public async Task<UserWithRoles?> GetUserByIdAsync(long id)
        {
            using var db = _dbFactory.Open();

            var user = await db.SingleAsync<User>(
                "SELECT * FROM user WHERE id = @id AND isdeleted = 0",
                new { id });

            if (user == null) return null;

            var roles = await db.ColumnAsync<string>(@"
                SELECT r.name
                FROM userrole ur
                JOIN role r ON ur.roleid = r.id
                WHERE ur.userid = @userId
            ", new { userId = id });

            return new UserWithRoles
            {
                User = user,
                Roles = roles.ToList()
            };
        }

        /// <summary>
        /// Inserts a new user record into the database and returns the generated ID.
        /// </summary>
        public async Task<long> InsertUserAsync(User user)
        {
            using var db = _dbFactory.Open();

            return await db.InsertAsync(user, selectIdentity: true);
        }

        /// <summary>
        /// Associates a user with a list of roles by their IDs.
        /// </summary>
        public async Task AssignRolesAsync(long userId, List<long> roleIds)
        {
            using var db = _dbFactory.Open();

            foreach (var roleId in roleIds)
            {
                await db.ExecuteSqlAsync(
                    "INSERT INTO userrole(userid, roleid) VALUES(@userId, @roleId)",
                    new { userId, roleId });
            }
        }

        /// <summary>
        /// Updates the core details and active status of a user record.
        /// </summary>
        public async Task UpdateUserAsync(long id, DTOUserUpdate request)
        {
            using var db = _dbFactory.Open();

            await db.ExecuteSqlAsync(@"
            UPDATE user
            SET name = @name, isactive = @isActive
            WHERE id = @id",
                new { id, name = request.Name, isActive = request.IsActive });
        }

        /// <summary>
        /// Completely replaces a user's assigned roles with a new set of role IDs.
        /// </summary>
        public async Task UpdateUserRolesAsync(long userId, List<long> roleIds)
        {
            using var db = _dbFactory.Open();

            await db.ExecuteSqlAsync("DELETE FROM userrole WHERE userid = @userId", new { userId });

            foreach (var roleId in roleIds)
            {
                await db.ExecuteSqlAsync(
                    "INSERT INTO userrole(userid, roleid) VALUES(@userId, @roleId)",
                    new { userId, roleId });
            }
        }

        /// <summary>
        /// Performs a soft delete on a user by setting their IsDeleted flag to true.
        /// </summary>
        public async Task DeleteUserAsync(long id)
        {
            using var db = _dbFactory.Open();

            await db.ExecuteSqlAsync(
                "UPDATE user SET isdeleted = 1 WHERE id = @id",
                new { id });
        }
    }
}
