using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance_Manager_MAL.DTO
{
    /// <summary>
    /// Update transaction status DTO
    /// </summary>
    public class DTOTransactionStatusUpdate
    {
        public string Status { get; set; } // approved / rejected
    }
}
