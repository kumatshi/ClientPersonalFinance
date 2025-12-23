using System.Net.Http.Headers;
using System.Text;
using ClientPersonalFinance.DTOs;
using Newtonsoft.Json;

namespace ClientPersonalFinance.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly HttpClient _httpClient;

        public CategoryService()
        {
            var handler = new HttpClientHandler();
#if DEBUG
            handler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
#endif

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://localhost:7068/api/"),
                Timeout = TimeSpan.FromSeconds(30)
            };

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

        public async Task<ApiResponse<IEnumerable<CategoryDto>>> GetCategoriesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("categories");
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResponse<IEnumerable<CategoryDto>>
                    {
                        Success = false,
                        Message = $"Ошибка: {response.StatusCode}"
                    };
                }

                return JsonConvert.DeserializeObject<ApiResponse<IEnumerable<CategoryDto>>>(responseString)
                    ?? new ApiResponse<IEnumerable<CategoryDto>>
                    {
                        Success = false,
                        Message = "Не удалось десериализовать ответ"
                    };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<CategoryDto>>
                {
                    Success = false,
                    Message = $"Ошибка: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<bool>> DeleteCategoryAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"categories/{id}");
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = $"Ошибка: {response.StatusCode}"
                    };
                }

                return JsonConvert.DeserializeObject<ApiResponse<bool>>(responseString)
                    ?? new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Не удалось десериализовать ответ"
                    };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"Ошибка: {ex.Message}"
                };
            }
        }

        // Заглушки для остальных методов
        public Task<ApiResponse<CategoryDto>> GetCategoryByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<IEnumerable<CategoryDto>>> GetIncomeCategoriesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<IEnumerable<CategoryDto>>> GetExpenseCategoriesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<CategoryDto>> CreateCategoryAsync(CreateCategoryDto category)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<CategoryDto>> UpdateCategoryAsync(int id, UpdateCategoryDto category)
        {
            throw new NotImplementedException();
        }
    }
}