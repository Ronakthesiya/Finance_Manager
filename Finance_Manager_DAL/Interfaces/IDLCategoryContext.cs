using System.Collections.Generic;
using System.Threading.Tasks;
using Finance_Manager_MAL.DTO;
using Finance_Manager_MAL.POCO;

namespace Finance_Manager_DAL.Interfaces
{
    public interface IDLCategoryContext
    {
        Task<(List<Category> Items, int TotalCount)> GetAllAsync(PaginationQuery query);
        Task<Category?> GetByIdAsync(long id);
        Task<long> InsertAsync(Category category);
        Task UpdateAsync(long id, DTOCategoryUpdate request);
        Task DeleteAsync(long id);
    }
}
