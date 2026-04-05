using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance_Manager_MAL.DTO
{
    public class DTOTransactionCreate
    {
        public string Title { get; set; }
        public decimal Amount { get; set; }

        public string Type { get; set; }

        public string Note { get; set; }

        public long CreatedBy { get; set; }

        public long CategoryId { get; set; }
    }
}
