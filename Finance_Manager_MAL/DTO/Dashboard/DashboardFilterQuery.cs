using System;

namespace Finance_Manager_MAL.DTO
{
    /// <summary>
    /// Represents a query for filtering dashboard statistics
    /// </summary>
    public class DashboardFilterQuery
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Limit { get; set; } = 5;
    }
}
