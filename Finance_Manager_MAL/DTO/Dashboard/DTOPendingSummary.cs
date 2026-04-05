using System;

namespace Finance_Manager_MAL.DTO
{
    /// <summary>
    /// Represents a summary of all pending transactions
    /// </summary>
    public class DTOPendingSummary
    {
        public int PendingCount { get; set; }
        public decimal TotalPendingAmount { get; set; }
    }
}
