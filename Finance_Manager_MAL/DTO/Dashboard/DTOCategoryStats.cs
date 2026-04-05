using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance_Manager_MAL.DTO
{
    /// <summary>
    /// Category-wise expense DTO
    /// </summary>
    public class DTOCategoryStats
    {
        public string Category { get; set; }
        public decimal Total { get; set; }
    }
}
