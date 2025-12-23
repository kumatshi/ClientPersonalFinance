using ClientPersonalFinance.DTOs;

namespace ClientPersonalFinance.Services
{
    public interface IApiService
    {
        Task<ApiResponse<string>> TestConnectionAsync();
        Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginRequestDto loginDto);
        Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterRequestDto registerDto);
        Task<ApiResponse<UserProfileDto>> GetUserProfileAsync();
        Task<bool> IsAuthenticatedAsync();
        Task LogoutAsync();
        string GetAccessToken();
    }
}