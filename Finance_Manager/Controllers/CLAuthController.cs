using Asp.Versioning;
using Finance_Manager_BAL.Interfaces;
using Finance_Manager_MAL.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Finance_Manager_API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/auth")]
    [ApiController]
    public class CLAuthController : ControllerBase
    {
        private readonly IBLAuthHandler _authHandler;

        public CLAuthController(IBLAuthHandler authHandler)
        {
            _authHandler = authHandler;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup(DTOSignupRequest request)
        {
            var result = await _authHandler.SignupAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("login")]
        [EnableRateLimiting("AuthPolicy")]
        public async Task<IActionResult> Login(DTOLoginRequest request)
        {
            var result = await _authHandler.LoginAsync(request);

            if (!result.Success)
                return Unauthorized(result.Message);

            return Ok(result);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] string refreshToken)
        {
            var result = await _authHandler.RefreshAsync(refreshToken);

            if (!result.Success)
                return Unauthorized(result.Message);

            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] string refreshToken)
        {
            await _authHandler.LogoutAsync(refreshToken);
            return Ok("Logged out successfully");
        }
    }
}
