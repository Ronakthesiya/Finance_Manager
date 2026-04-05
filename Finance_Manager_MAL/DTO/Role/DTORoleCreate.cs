using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance_Manager_MAL.DTO
{
    public class DTORoleCreate
    {
        public string Name { get; set; }
        public string Slug { get; set; }

        public List<long> PermissionIds { get; set; } = new();
    }
}
