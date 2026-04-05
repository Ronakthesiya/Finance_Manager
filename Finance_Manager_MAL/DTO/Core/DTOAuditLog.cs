using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance_Manager_MAL.DTO
{
    public class DTOAuditLog
    {
        public long Id { get; set; }

        public string UserName { get; set; }

        public string Action { get; set; }
        public string Entity { get; set; }

        public long EntityId { get; set; }

        public string? OldValue { get; set; }
        public string? NewValue { get; set; }

        public string? IpAddress { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
