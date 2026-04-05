using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance_Manager_MAL.POCO
{
    public class UserRole
    {
        public long Id { get; set; } = 0;

        public long UserId { get; set; } = 0;
        public User User { get; set; }

        public long RoleId { get; set; } = 0;
        public Role Role { get; set; }

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    }
}
