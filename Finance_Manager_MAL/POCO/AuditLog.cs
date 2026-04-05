using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance_Manager_MAL.POCO
{
    public class AuditLog
    {
        public long Id { get; set; } = 0;

        public long UserId { get; set; } = 0;
        public User User { get; set; }

        public string Action { get; set; } = string.Empty;
        public string Entity { get; set; } = string.Empty;

        public long EntityId { get; set; } = 0;

        public string? OldValue { get; set; }
        public string? NewValue { get; set; }

        public string? IpAddress { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
