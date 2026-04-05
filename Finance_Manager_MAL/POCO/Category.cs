using Finance_Manager_MAL.Enums;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Finance_Manager_MAL.POCO
{
    public class Category
    {
        public long Id { get; set; } = 0;
        public string Name { get; set; } = string.Empty;

        public CategoryType Type { get; set; }

        public string? Color { get; set; }

        public long CreatedBy { get; set; } = 0;

        [Ignore]
        public User CreatedByUser { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Ignore]
        public ICollection<Transaction> Transactions { get; set; }
    }
}
