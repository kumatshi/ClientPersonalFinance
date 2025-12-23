using System.Net.Http.Headers;
using System.Text;
using ClientPersonalFinance.DTOs;
using Newtonsoft.Json;

namespace ClientPersonalFinance.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly HttpClient _httpClient;

        public TransactionService()
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

        public async Task<ApiResponse<IEnumerable<TransactionDto>>> GetTransactionsAsync(int page = 1, int pageSize = 20)
        {
            try
            {
                Console.WriteLine($"[DEBUG] Запрос транзакций: страница {page}");

                var response = await _httpClient.GetAsync($"transactions?page={page}&pageSize={pageSize}");
                var responseString = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"[DEBUG] Ответ: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResponse<IEnumerable<TransactionDto>>
                    {
                        Success = false,
                        Message = $"Ошибка сервера: {response.StatusCode}"
                    };
                }

                var result = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<TransactionDto>>>(responseString);

                if (result == null)
                {
                    return new ApiResponse<IEnumerable<TransactionDto>>
                    {
                        Success = false,
                        Message = "Не удалось обработать ответ сервера"
                    };
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Ошибка при получении транзакций: {ex.Message}");
                return new ApiResponse<IEnumerable<TransactionDto>>
                {
                    Success = false,
                    Message = $"Ошибка соединения: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<FinancialSummaryDto>> GetFinancialSummaryAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var url = "transactions/summary";
                if (startDate.HasValue || endDate.HasValue)
                {
                    url += "?";
                    if (startDate.HasValue) url += $"startDate={startDate.Value:yyyy-MM-dd}&";
                    if (endDate.HasValue) url += $"endDate={endDate.Value:yyyy-MM-dd}";
                }

                var response = await _httpClient.GetAsync(url);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResponse<FinancialSummaryDto>
                    {
                        Success = false,
                        Message = $"Ошибка: {response.StatusCode}"
                    };
                }

                return JsonConvert.DeserializeObject<ApiResponse<FinancialSummaryDto>>(responseString)
                    ?? new ApiResponse<FinancialSummaryDto>
                    {
                        Success = false,
                        Message = "Не удалось десериализовать ответ"
                    };
            }
            catch (Exception ex)
            {
                return new ApiResponse<FinancialSummaryDto>
                {
                    Success = false,
                    Message = $"Ошибка: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<bool>> DeleteTransactionAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"transactions/{id}");
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

        public Task<ApiResponse<TransactionDto>> GetTransactionByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse<TransactionDto>> CreateTransactionAsync(CreateTransactionDto transaction)
        {
            try
            {
                var json = JsonConvert.SerializeObject(transaction);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("transactions", content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResponse<TransactionDto>
                    {
                        Success = false,
                        Message = $"Ошибка: {response.StatusCode}"
                    };
                }

                return JsonConvert.DeserializeObject<ApiResponse<TransactionDto>>(responseString)
                    ?? new ApiResponse<TransactionDto>
                    {
                        Success = false,
                        Message = "Не удалось десериализовать ответ"
                    };
            }
            catch (Exception ex)
            {
                return new ApiResponse<TransactionDto>
                {
                    Success = false,
                    Message = $"Ошибка: {ex.Message}"
                };
            }
        }

        public Task<ApiResponse<TransactionDto>> UpdateTransactionAsync(int id, UpdateTransactionDto transaction)
        {
            throw new NotImplementedException();
        }
    }
}