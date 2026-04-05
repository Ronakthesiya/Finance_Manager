using System;

namespace Finance_Manager_MAL.DTO
{
    /// <summary>
    /// Represents daily income and expense statistics
    /// </summary>
    public class DTODailyStats
    {
        public string Date { get; set; } = string.Empty;
        public decimal Income { get; set; }
        public decimal Expense { get; set; }
    }
}
