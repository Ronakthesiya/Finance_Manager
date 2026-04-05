using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance_Manager_MAL.DTO
{
    /// <summary>
    /// DTO for updating role
    /// </summary>
    public class DTORoleUpdate
    {
        public string Name { get; set; }
        public string Slug { get; set; }
        public bool IsActive { get; set; }
    }
}
