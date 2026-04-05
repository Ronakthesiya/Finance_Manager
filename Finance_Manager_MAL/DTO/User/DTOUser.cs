using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance_Manager_MAL.DTO
{
    public class DTOUser
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public bool IsActive { get; set; }

        public List<string> Roles { get; set; } = new();
    }
}
