using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance_Manager_MAL.DTO
{
    public class DTORole
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }

        public bool IsActive { get; set; }

        public List<string> Permissions { get; set; } = new();
    }
}
