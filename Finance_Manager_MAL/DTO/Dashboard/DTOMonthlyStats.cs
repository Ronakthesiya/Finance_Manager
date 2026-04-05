using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance_Manager_MAL.DTO
{
    /// <summary>
    /// Monthly chart DTO
    /// </summary>
    public class DTOMonthlyStats
    {
        public string Month { get; set; }
        public decimal Income { get; set; }
        public decimal Expense { get; set; }
    }
}
