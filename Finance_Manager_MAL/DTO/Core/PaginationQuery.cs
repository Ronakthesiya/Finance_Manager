using System;

namespace Finance_Manager_MAL.DTO
{
    /// <summary>
    /// Represents standard pagination parameters 
    /// </summary>
    public class PaginationQuery
    {
        private int _pageSize = 10;
        public int PageNumber { get; set; } = 1;
        
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > 100) ? 100 : value;
        }
    }
}
