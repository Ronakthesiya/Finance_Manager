using Finance_Manager_MAL.Enums;
using System;

namespace Finance_Manager_MAL.DTO
{
    /// <summary>
    /// Represents filtering and pagination parameters for transactions
    /// </summary>
    public class TransactionFilterQuery : PaginationQuery
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? CategoryId { get; set; }
        public TransactionType? Type { get; set; }
        public TransactionStatus? Status { get; set; }
        public string? SearchTerm { get; set; }
    }
}
