using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Finance_Manager_MAL.POCO
{
    public class User
    {
        public long Id { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Ignore]
        public ICollection<UserRole> UserRoles { get; set; }
        [Ignore]
        public ICollection<Transaction> CreatedTransactions { get; set; }
        [Ignore]
        public ICollection<Transaction> ApprovedTransactions { get; set; }
    }
}
