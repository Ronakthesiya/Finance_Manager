using Finance_Manager_Core.Services;
using Finance_Manager_DAL.Context;
using Finance_Manager_DAL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace Finance_Manager_BAL
{

    /// <summary>
    /// Handles RBAC validation
    /// </summary>
    public class PermissionService
    {
        private readonly RedisService _redis;
        private readonly IDLAuthContext _context;
        private readonly IConfiguration _config;

        public PermissionService(IConfiguration config, RedisService redis, IDLAuthContext context)
        {
            _config = config;
            _redis = redis;
            _context = context;
        }

        public async Task<bool> ValidateAsync(HttpContext httpContext, string requiredPermission)
        {
            //// 1. Get token from Authorization header
            //var authHeader = httpContext.Request.Headers["Authorization"].ToString();

            //if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            //    return false;

            //var token = authHeader.Replace("Bearer ", "");

            //// 2. Validate & decode JWT
            //var handler = new JwtSecurityTokenHandler();

            //var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]); // ⚠️ move to config

            //ClaimsPrincipal principal;

            //try
            //{
            //    principal = handler.ValidateToken(token, new TokenValidationParameters
            //    {
            //        ValidateIssuer = false,
            //        ValidateAudience = false,
            //        ValidateIssuerSigningKey = true,
            //        IssuerSigningKey = new SymmetricSecurityKey(key),
            //        ValidateLifetime = true,
            //        ClockSkew = TimeSpan.Zero
            //    }, out _);
            //}
            //catch
            //{
            //    return false;
            //}

            // 3. Extract userId from claims
            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return false;

            // 4. Validate session in Redis
            var sessionKeyPattern = $"session:{userId}:*";

            var sessionKeys = await _redis.GetKeysAsync(sessionKeyPattern);

            var sessionKey = sessionKeys?.FirstOrDefault();

            if (string.IsNullOrEmpty(sessionKey))
                return false;

            var session = await _redis.GetAsync(sessionKey);

            if (session == null)
                return false;

            // 5. Get permissions from cache
            var cacheKey = $"permissions:{userId}";
            var permissions = await _redis.GetAsync<List<string>>(cacheKey);

            if (permissions == null)
            {
                permissions = await _context.GetUserPermissionsAsync(long.Parse(userId));

                await _redis.SetAsync(cacheKey, permissions, 600);
            }

            // 6. Check permission
            return permissions.Contains(requiredPermission);
        }
    }
}
