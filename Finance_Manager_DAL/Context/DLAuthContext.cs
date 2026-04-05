using Finance_Manager_MAL.POCO;
using Finance_Manager_DAL.Interfaces;
using ServiceStack.Data;
using ServiceStack.OrmLite;

namespace Finance_Manager_DAL.Context
{

    public class DLAuthContext : IDLAuthContext
    {
        private readonly IDbConnectionFactory _dbFactory;

        public DLAuthContext(IDbConnectionFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        /// <summary>
        /// Retrieves a user record by their matching email address.
        /// </summary>
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            using var db = _dbFactory.Open();

            return await db.SingleAsync<User>(
                "SELECT * FROM user WHERE email = @email AND isdeleted = 0",
                new { email });
        }

        /// <summary>
        /// Retrieves a user record by their unique identifier.
        /// </summary>
        public async Task<User?> GetUserByIdAsync(long id)
        {
            using var db = _dbFactory.Open();

            return await db.SingleAsync<User>(
                "SELECT * FROM user WHERE id = @id",
                new { id });
        }

        /// <summary>
        /// Retrieves a list of role names associated with a specific user.
        /// </summary>
        public async Task<List<string>> GetUserRolesAsync(long userId)
        {
            using var db = _dbFactory.Open();

            var sql = @"
            SELECT r.name
            FROM userrole ur
            JOIN role r ON ur.roleid = r.id
            WHERE ur.userid = @userId
        ";

            return (await db.ColumnAsync<string>(sql, new { userId })).ToList();
        }

        /// <summary>
        /// Retrieves a unique list of permission slugs associated with all roles of a user.
        /// </summary>
        public async Task<List<string>> GetUserPermissionsAsync(long userId)
        {
            using var db = _dbFactory.Open();

            var sql = @"
                SELECT DISTINCT p.slug
                FROM userrole ur
                JOIN rolepermission rp ON ur.roleid = rp.roleid
                JOIN permission p ON rp.permissionid = p.id
                WHERE ur.userid = @userId
            ";

            return (await db.ColumnAsync<string>(sql, new { userId })).ToList();
        }
    }
}
