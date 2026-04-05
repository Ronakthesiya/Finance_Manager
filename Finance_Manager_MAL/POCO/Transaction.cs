using Finance_Manager_MAL.Enums;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance_Manager_MAL.POCO
{
    public class Transaction
    {
        public long Id { get; set; } = 0;
        public string Title { get; set; } = string.Empty;
        public decimal Amount { get; set; } = decimal.Zero;

        public TransactionType Type { get; set; }
        public TransactionStatus Status { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        public string? Note { get; set; }

        public long CategoryId { get; set; } = 0;

        [Ignore]
        public Category Category { get; set; }

        public long CreatedBy { get; set; } = 0;

        [Ignore]
        public User CreatedByUser { get; set; }

        public long? ApprovedBy { get; set; }

        [Ignore]
        public User? ApprovedByUser { get; set; }

        public DateTime? ApprovedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
