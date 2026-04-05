using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance_Manager_MAL.POCO
{
    public class RolePermission
    {
        public long Id { get; set; } = 0;

        public long RoleId { get; set; } = 0;
        public Role Role { get; set; }

        public long PermissionId { get; set; } = 0;
        public Permission Permission { get; set; }

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    }
}
