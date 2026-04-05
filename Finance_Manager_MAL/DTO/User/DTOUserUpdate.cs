using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance_Manager_MAL.DTO
{
    public class DTOUserUpdate
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public List<long> RoleIds { get; set; } = new();
    }
}
