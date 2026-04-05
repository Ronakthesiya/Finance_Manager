using Finance_Manager_BAL.Interfaces;
using Finance_Manager_MAL.DTO;
using Finance_Manager_MAL.POCO;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto;
using Finance_Manager_DAL.Context;
using Finance_Manager_DAL.Interfaces;
using Finance_Manager_Core.Interface;
using Finance_Manager_DAL.Models;

namespace Finance_Manager_BAL
{
    /// <summary>
    /// Handles business logic for user operations
    /// </summary>
    public class BLUserHandler : IBLUserHandler
    {
        private readonly IDLUserContext _context;
        private readonly IDTOPOCOMapper _mapper;

        public BLUserHandler(IDLUserContext context, IDTOPOCOMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all registered users along with their roles using pagination
        /// </summary>
        public async Task<ApiResponse<PagedData<DTOUser>>> GetAllUsersAsync(PaginationQuery query)
        {
            var (users, totalCount) = await _context.GetAllUsersAsync(query);

            var data = users.Select(u =>
            {
                var dto = _mapper.Map<User,DTOUser>(u.User);
                dto.Roles = u.Roles;
                return dto;
            }).ToList();

            var pagedData = new PagedData<DTOUser>
            {
                Items = data,
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };

            return new ApiResponse<PagedData<DTOUser>>
            {
                Success = true,
                Data = pagedData
            };
        }

        /// <summary>
        /// Retrieves a specific user and their roles by ID
        /// </summary>
        public async Task<ApiResponse<DTOUser>> GetUserByIdAsync(long id)
        {
            var user = await _context.GetUserByIdAsync(id);

            if (user == null)
            {
                return new ApiResponse<DTOUser>
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            var dto = _mapper.Map<User,DTOUser>(user.User);
            dto.Roles = user.Roles;

            return new ApiResponse<DTOUser>
            {
                Success = true,
                Data = dto
            };
        }

        /// <summary>
        /// Creates a new user, hashes their password, and assigns roles
        /// </summary>
        public async Task<ApiResponse<DTOUser>> CreateUserAsync(DTOUserCreate request)
        {
            var user = _mapper.Map<DTOUserCreate,User>(request);

            // 🔐 Hash password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            var userId = await _context.InsertUserAsync(user);

            await _context.AssignRolesAsync(userId, request.RoleIds);

            var createdUser = await _context.GetUserByIdAsync(userId);

            var dto = _mapper.Map<User,DTOUser>(createdUser.User);
            dto.Roles = createdUser.Roles;

            return new ApiResponse<DTOUser>
            {
                Success = true,
                Message = "User created successfully",
                Data = dto
            };
        }

        /// <summary>
        /// Updates a user's details and their assigned roles
        /// </summary>
        public async Task<ApiResponse<string>> UpdateUserAsync(long id, DTOUserUpdate request)
        {
            var exists = await _context.GetUserByIdAsync(id);

            if (exists == null)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            await _context.UpdateUserAsync(id, request);

            await _context.UpdateUserRolesAsync(id, request.RoleIds);

            return new ApiResponse<string>
            {
                Success = true,
                Message = "User updated successfully"
            };
        }

        /// <summary>
        /// Soft deletes a user from the system
        /// </summary>
        public async Task<ApiResponse<string>> DeleteUserAsync(long id)
        {
            var exists = await _context.GetUserByIdAsync(id);

            if (exists == null)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            await _context.DeleteUserAsync(id);

            return new ApiResponse<string>
            {
                Success = true,
                Message = "User deleted successfully"
            };
        }
    }
}