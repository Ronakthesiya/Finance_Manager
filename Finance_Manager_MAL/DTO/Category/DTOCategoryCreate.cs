using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance_Manager_MAL.DTO
{
    public class DTOCategoryCreate
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string? Color { get; set; }

        public long CreatedBy { get; set; }
    }
}
