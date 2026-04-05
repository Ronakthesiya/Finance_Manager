using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance_Manager_MAL.DTO
{
    public class DTOCategory
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public long CreatedBy { get; set; }
        public string? Color { get; set; }
        public bool IsActive { get; set; }
    }
}
