using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance_Manager_MAL.DTO
{
    /// <summary>
    /// Permission response DTO
    /// </summary>
    public class DTOPermission
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Module { get; set; }
    }
}
