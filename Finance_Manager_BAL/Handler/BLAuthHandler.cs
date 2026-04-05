using Finance_Manager_BAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Finance_Manager_MAL.DTO;
using Finance_Manager_MAL.POCO;
using Finance_Manager_DAL.Context;
using Finance_Manager_DAL.Interfaces;
using Finance_Manager_Core.Services;
namespace Finance_Manager_BAL
{
    public class BLAuthHandler : IBLAuthHandler
    {
        private readonly IDLAuthContext _context;
        private readonly IDLUserContext _userContext;
        private readonly JwtService _jwtService;
        private readonly RedisService _redis;

        public BLAuthHandler(
            IDLAuthContext context,
            IDLUserContext userContext,
            JwtService jwtService,
            RedisService redis)
        {
            _context = context;
            _userContext = userContext;
            _jwtService = jwtService;
            _redis = redis;
        }

        // 📝 SIGNUP
        /// <summary>
        /// Registers a new user
        /// </summary>
        public async Task<ApiResponse<string>> SignupAsync(DTOSignupRequest request)
        {
            var existingUser = await _context.GetUserByEmailAsync(request.Email);
            
            if (existingUser != null)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Email already registered"
                };
            }

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var userId = await _userContext.InsertUserAsync(user);

            // Assign default role = 4
            await _userContext.AssignRolesAsync(userId, new List<long> { 4 });

            return new ApiResponse<string>
            {
                Success = true,
                Message = "User registered successfully"
            };
        }

        // 🔐 LOGIN
        /// <summary>
        /// Authenticates a user and returns JWT tokens
        /// </summary>
        public async Task<ApiResponse<DTOAuthResponse>> LoginAsync(DTOLoginRequest request)
        {
            var user = await _context.GetUserByEmailAsync(request.Email);
            
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return new ApiResponse<DTOAuthResponse>
                {
                    Success = false,
                    Message = "Invalid credentials"
                };
            }

            var roles = await _context.GetUserRolesAsync(user.Id);

            var accessToken = _jwtService.GenerateAccessToken(user, roles);

            var refreshToken = Guid.NewGuid().ToString();
            var redisKey = $"session:{user.Id}:{refreshToken}";

            await _redis.SetAsync(redisKey, user.Id.ToString(), 24 * 60 * 60);

            return new ApiResponse<DTOAuthResponse>
            {
                Success = true,
                Data = new DTOAuthResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                }
            };
        }

        // 🔁 REFRESH
        /// <summary>
        /// Refreshes the access token using a refresh token
        /// </summary>
        public async Task<ApiResponse<DTOAuthResponse>> RefreshAsync(string refreshToken)
        {
            // Extract userId from token (you may pass separately also)
            var keys = await _redis.GetKeysAsync($"session:*:{refreshToken}");

            if (!keys.Any())
            {
                return new ApiResponse<DTOAuthResponse>
                {
                    Success = false,
                    Message = "Invalid refresh token"
                };
            }

            var key = keys.First();
            var userId = key.Split(':')[1];

            await _redis.RemoveAsync(key);

            var newRefreshToken = Guid.NewGuid().ToString();
            var newKey = $"session:{userId}:{newRefreshToken}";

            await _redis.SetAsync(newKey, userId, 7 * 24 * 60 * 60);

            var user = await _context.GetUserByIdAsync(long.Parse(userId));
            var roles = await _context.GetUserRolesAsync(user.Id);

            var newAccessToken = _jwtService.GenerateAccessToken(user, roles);

            return new ApiResponse<DTOAuthResponse>
            {
                Success = true,
                Data = new DTOAuthResponse
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                }
            };
        }

        // 🚪 LOGOUT
        /// <summary>
        /// Logs out a user by invalidating the refresh token
        /// </summary>
        public async Task LogoutAsync(string refreshToken)
        {
            var keys = await _redis.GetKeysAsync($"session:*:{refreshToken}");

            foreach (var key in keys)
            {
                await _redis.RemoveAsync(key);
            }
        }
    }
}
