using System.Net.Http.Headers;
using System.Text;
using ClientPersonalFinance.DTOs;
using Newtonsoft.Json;

namespace ClientPersonalFinance.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:7068/api/";

        public ApiService()
        {
#if DEBUG
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            _httpClient = new HttpClient(handler);
#else
            _httpClient = new HttpClient();
#endif

            _httpClient.BaseAddress = new Uri(BaseUrl);
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            InitializeHeaders();
        }

        private async void InitializeHeaders()
        {
            var token = await SecureStorage.GetAsync("AuthToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<ApiResponse<string>> TestConnectionAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("swagger/v1/swagger.json");
                var responseString = await response.Content.ReadAsStringAsync();

                return new ApiResponse<string>
                {
                    Success = response.IsSuccessStatusCode,
                    Message = response.IsSuccessStatusCode ? "Соединение установлено" : $"Ошибка: {response.StatusCode}",
                    Data = responseString
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = $"Ошибка соединения: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginRequestDto loginDto)
        {
            try
            {
                var json = JsonConvert.SerializeObject(loginDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("auth/login", content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResponse<AuthResponseDto>
                    {
                        Success = false,
                        Message = $"Ошибка: {response.StatusCode}"
                    };
                }

                var result = JsonConvert.DeserializeObject<ApiResponse<AuthResponseDto>>(responseString);

                if (result?.Success == true && result.Data != null)
                {
                    await SecureStorage.SetAsync("AuthToken", result.Data.AccessToken);
                    await SecureStorage.SetAsync("RefreshToken", result.Data.RefreshToken);
                    await SecureStorage.SetAsync("UserId", result.Data.UserId.ToString());

                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", result.Data.AccessToken);
                }

                return result ?? new ApiResponse<AuthResponseDto>
                {
                    Success = false,
                    Message = "Не удалось обработать ответ сервера"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<AuthResponseDto>
                {
                    Success = false,
                    Message = $"Ошибка соединения: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterRequestDto registerDto)
        {
            try
            {
                var json = JsonConvert.SerializeObject(registerDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("auth/register", content);
                var responseString = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<ApiResponse<AuthResponseDto>>(responseString);

                if (result?.Success == true && result.Data != null)
                {
                    await SecureStorage.SetAsync("AuthToken", result.Data.AccessToken);
                    await SecureStorage.SetAsync("RefreshToken", result.Data.RefreshToken);
                    await SecureStorage.SetAsync("UserId", result.Data.UserId.ToString());

                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", result.Data.AccessToken);
                }

                return result ?? new ApiResponse<AuthResponseDto>
                {
                    Success = false,
                    Message = "Ошибка при регистрации"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<AuthResponseDto>
                {
                    Success = false,
                    Message = $"Ошибка соединения: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<UserProfileDto>> GetUserProfileAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("auth/profile");
                var responseString = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<ApiResponse<UserProfileDto>>(responseString)
                    ?? new ApiResponse<UserProfileDto>
                    {
                        Success = false,
                        Message = "Ошибка при получении профиля"
                    };
            }
            catch (Exception ex)
            {
                return new ApiResponse<UserProfileDto>
                {
                    Success = false,
                    Message = $"Ошибка соединения: {ex.Message}"
                };
            }
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await SecureStorage.GetAsync("AuthToken");
            return !string.IsNullOrEmpty(token);
        }

        public async Task LogoutAsync()
        {
            try
            {
                await SecureStorage.SetAsync("AuthToken", string.Empty);
                await SecureStorage.SetAsync("RefreshToken", string.Empty);
                await SecureStorage.SetAsync("UserId", string.Empty);

                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
            catch (Exception)
            {
                // Игнорируем ошибки при выходе
            }
        }

        public string GetAccessToken()
        {
            return SecureStorage.GetAsync("AuthToken").Result ?? string.Empty;
        }
    }
}