using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance_Manager_MAL.DTO
{
    public class DTOUserCreate
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public List<long> RoleIds { get; set; } = new();
    }
}
