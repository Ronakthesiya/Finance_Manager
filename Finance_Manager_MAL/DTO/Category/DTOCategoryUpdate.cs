using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance_Manager_MAL.DTO
{
    /// <summary>
    /// Update category DTO
    /// </summary>
    public class DTOCategoryUpdate
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Color { get; set; }
        public bool IsActive { get; set; }
    }
}
