using Finance_Manager_Core.Interface;
using Finance_Manager_DAL.Context;
using Finance_Manager_DAL.Interfaces;
using Finance_Manager_MAL.DTO;
using Finance_Manager_MAL.Enums;
using Finance_Manager_MAL.POCO;
using Finance_Manager_BAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance_Manager_BAL
{
    /// <summary>
    /// Handles business logic for transactions
    /// </summary>
    public class BLTransactionHandler : IBLTransactionHandler
    {
        private readonly IDLTransactionContext _context;
        private readonly IDTOPOCOMapper _mapper;

        public BLTransactionHandler(IDLTransactionContext context, IDTOPOCOMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all transactions with filtering and pagination
        /// </summary>
        public async Task<ApiResponse<PagedData<DTOTransaction>>> GetAllAsync(TransactionFilterQuery query)
        {
            var (list, totalCount) = await _context.GetAllAsync(query);

            var data = list.Select(x => _mapper.Map<Transaction, DTOTransaction>(x)).ToList();

            var pagedData = new PagedData<DTOTransaction>
            {
                Items = data,
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };

            return new ApiResponse<PagedData<DTOTransaction>>
            {
                Success = true,
                Data = pagedData
            };
        }

        /// <summary>
        /// Retrieves a transaction by its ID
        /// </summary>
        public async Task<ApiResponse<DTOTransaction>> GetByIdAsync(long id)
        {
            var txn = await _context.GetByIdAsync(id);

            if (txn == null)
                return new ApiResponse<DTOTransaction> { Success = false, Message = "Not found" };

            return new ApiResponse<DTOTransaction>
            {
                Success = true,
                Data = _mapper.Map<Transaction, DTOTransaction>(txn)
            };
        }

        /// <summary>
        /// Creates a new transaction
        /// </summary>
        public async Task<ApiResponse<DTOTransaction>> CreateAsync(DTOTransactionCreate request)
        {
            if (request.Amount <= 0)
            {
                return new ApiResponse<DTOTransaction>
                {
                    Success = false,
                    Message = "Amount must be greater than 0"
                };
            }

            var entity = _mapper.Map<DTOTransactionCreate, Transaction>(request);
            entity.Status = TransactionStatus.Pending;

            var id = await _context.InsertAsync(entity);

            var created = await _context.GetByIdAsync(id);

            return new ApiResponse<DTOTransaction>
            {
                Success = true,
                Message = "Transaction created",
                Data = _mapper.Map<Transaction, DTOTransaction>(created)
            };
        }

        /// <summary>
        /// Updates an existing transaction's details
        /// </summary>
        public async Task<ApiResponse<string>> UpdateAsync(long id, DTOTransactionUpdate request)
        {
            var exists = await _context.GetByIdAsync(id);

            if (exists == null)
                return new ApiResponse<string> { Success = false, Message = "Not found" };

            await _context.UpdateAsync(id, request);

            return new ApiResponse<string>
            {
                Success = true,
                Message = "Updated successfully"
            };
        }

        /// <summary>
        /// Updates the status of a transaction (e.g., approve/reject)
        /// </summary>
        public async Task<ApiResponse<string>> UpdateStatusAsync(long id, DTOTransactionStatusUpdate request)
        {
            if (request.Status != "approved" && request.Status != "rejected")
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Invalid status"
                };
            }

            var exists = await _context.GetByIdAsync(id);

            if (exists == null)
                return new ApiResponse<string> { Success = false, Message = "Not found" };

            await _context.UpdateStatusAsync(id, request.Status);

            return new ApiResponse<string>
            {
                Success = true,
                Message = $"Transaction {request.Status}"
            };
        }

        /// <summary>
        /// Soft deletes a transaction
        /// </summary>
        public async Task<ApiResponse<string>> DeleteAsync(long id)
        {
            var exists = await _context.GetByIdAsync(id);

            if (exists == null)
                return new ApiResponse<string> { Success = false, Message = "Not found" };

            await _context.DeleteAsync(id);

            return new ApiResponse<string>
            {
                Success = true,
                Message = "Deleted successfully"
            };
        }
    }
}
