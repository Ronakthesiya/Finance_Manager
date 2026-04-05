using System.Threading.Tasks;
using Finance_Manager_MAL.DTO;

namespace Finance_Manager_BAL.Interfaces
{
    public interface IBLAuthHandler
    {
        Task<ApiResponse<string>> SignupAsync(DTOSignupRequest request);
        Task<ApiResponse<DTOAuthResponse>> LoginAsync(DTOLoginRequest request);
        Task<ApiResponse<DTOAuthResponse>> RefreshAsync(string refreshToken);
        Task LogoutAsync(string refreshToken);
    }
}
