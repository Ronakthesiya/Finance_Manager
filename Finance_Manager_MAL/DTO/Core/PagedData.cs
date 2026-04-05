using System;
using System.Collections.Generic;

namespace Finance_Manager_MAL.DTO
{
    /// <summary>
    /// Represents a paginated list of items with metadata
    /// </summary>
    public class PagedData<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
    }
}
