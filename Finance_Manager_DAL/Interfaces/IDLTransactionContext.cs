using System.Collections.Generic;
using System.Threading.Tasks;
using Finance_Manager_MAL.DTO;
using Finance_Manager_MAL.POCO;

namespace Finance_Manager_DAL.Interfaces
{
    public interface IDLTransactionContext
    {
        Task<(List<Transaction> Items, int TotalCount)> GetAllAsync(TransactionFilterQuery query);
        Task<Transaction?> GetByIdAsync(long id);
        Task<long> InsertAsync(Transaction txn);
        Task UpdateAsync(long id, DTOTransactionUpdate request);
        Task UpdateStatusAsync(long id, string status);
        Task DeleteAsync(long id);
    }
}
