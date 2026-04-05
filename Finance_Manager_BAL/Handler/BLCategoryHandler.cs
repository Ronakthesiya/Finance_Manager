using Finance_Manager_BAL.Interfaces;
using Finance_Manager_Core.Interface;
using Finance_Manager_DAL.Context;
using Finance_Manager_DAL.Interfaces;
using Finance_Manager_MAL.DTO;
using Finance_Manager_MAL.POCO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance_Manager_BAL
{
    /// <summary>
    /// Handles business logic for categories
    /// </summary>
    public class BLCategoryHandler : IBLCategoryHandler
    {
        private readonly IDLCategoryContext _context;
        private readonly IDTOPOCOMapper _mapper;

        public BLCategoryHandler(IDLCategoryContext context, IDTOPOCOMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all categories with pagination support
        /// </summary>
        public async Task<ApiResponse<PagedData<DTOCategory>>> GetAllAsync(PaginationQuery query)
        {
            var (categories, totalCount) = await _context.GetAllAsync(query);

            var data = categories
                .Select(c => _mapper.Map<Category, DTOCategory>(c))
                .ToList();

            var pagedData = new PagedData<DTOCategory>
            {
                Items = data,
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };

            return new ApiResponse<PagedData<DTOCategory>>
            {
                Success = true,
                Data = pagedData
            };
        }

        /// <summary>
        /// Retrieves a specific category by its unique ID
        /// </summary>
        public async Task<ApiResponse<DTOCategory>> GetByIdAsync(long id)
        {
            var category = await _context.GetByIdAsync(id);

            if (category == null)
            {
                return new ApiResponse<DTOCategory>
                {
                    Success = false,
                    Message = "Category not found"
                };
            }

            var dto = _mapper.Map<Category, DTOCategory>(category);

            return new ApiResponse<DTOCategory>
            {
                Success = true,
                Data = dto
            };
        }

        /// <summary>
        /// Creates a new category
        /// </summary>
        public async Task<ApiResponse<DTOCategory>> CreateAsync(DTOCategoryCreate request)
        {
            var category = _mapper.Map<DTOCategoryCreate, Category>(request);

            var id = await _context.InsertAsync(category);

            var created = await _context.GetByIdAsync(id);

            var dto = _mapper.Map<Category, DTOCategory>(created);

            return new ApiResponse<DTOCategory>
            {
                Success = true,
                Message = "Category created successfully",
                Data = dto
            };
        }

        /// <summary>
        /// Updates an existing category's details
        /// </summary>
        public async Task<ApiResponse<string>> UpdateAsync(long id, DTOCategoryUpdate request)
        {
            var exists = await _context.GetByIdAsync(id);

            if (exists == null)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Category not found"
                };
            }

            await _context.UpdateAsync(id, request);

            return new ApiResponse<string>
            {
                Success = true,
                Message = "Category updated successfully"
            };
        }

        /// <summary>
        /// Soft deletes a category by its ID
        /// </summary>
        public async Task<ApiResponse<string>> DeleteAsync(long id)
        {
            var exists = await _context.GetByIdAsync(id);

            if (exists == null)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Category not found"
                };
            }

            await _context.DeleteAsync(id);

            return new ApiResponse<string>
            {
                Success = true,
                Message = "Category deleted successfully"
            };
        }
    }
}
