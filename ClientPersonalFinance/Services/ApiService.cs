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
                Console.WriteLine($"[DEBUG] Тестируем соединение с: {BaseUrl}");

                var response = await _httpClient.GetAsync("swagger/v1/swagger.json");
                var responseString = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"[DEBUG] Статус код: {response.StatusCode}");
                Console.WriteLine($"[DEBUG] Ответ (первые 200 символов): {responseString.Substring(0, Math.Min(200, responseString.Length))}...");

                return new ApiResponse<string>
                {
                    Success = response.IsSuccessStatusCode,
                    Message = response.IsSuccessStatusCode ?
                        "Соединение установлено" :
                        $"Ошибка: {response.StatusCode} - {responseString}",
                    Data = responseString
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Тест соединения: {ex.Message}");
                Console.WriteLine($"[ERROR] Подробности: {ex.InnerException?.Message}");

                try
                {
                    var simpleClient = new HttpClient()
                    {
                        BaseAddress = new Uri("https://localhost:7068/"),
                        Timeout = TimeSpan.FromSeconds(5)
                    };

                    var simpleResponse = await simpleClient.GetAsync("");
                    var simpleResponseString = await simpleResponse.Content.ReadAsStringAsync();

                    return new ApiResponse<string>
                    {
                        Success = false,
                        Message = $"API доступен, но возможно неверный путь. Ответ: {simpleResponseString.Substring(0, Math.Min(100, simpleResponseString.Length))}..."
                    };
                }
                catch (Exception ex2)
                {
                    return new ApiResponse<string>
                    {
                        Success = false,
                        Message = $"Ошибка соединения: {ex.Message}\nВнутренняя ошибка: {ex2.Message}"
                    };
                }
            }
        }

        public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginRequestDto loginDto)
        {
            try
            {
                Console.WriteLine($"[DEBUG] Отправка запроса на: {BaseUrl}auth/login");

                var json = JsonConvert.SerializeObject(loginDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("auth/login", content);
                var responseString = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"[DEBUG] Статус код: {response.StatusCode}");
                Console.WriteLine($"[DEBUG] Ответ: {responseString}");

                if (!response.IsSuccessStatusCode)
                {
                    try
                    {
                        var errorResult = JsonConvert.DeserializeObject<ApiResponse<AuthResponseDto>>(responseString);
                        return errorResult ?? new ApiResponse<AuthResponseDto>
                        {
                            Success = false,
                            Message = $"Ошибка сервера: {response.StatusCode}"
                        };
                    }
                    catch
                    {
                        return new ApiResponse<AuthResponseDto>
                        {
                            Success = false,
                            Message = $"Ошибка: {response.StatusCode}. Ответ: {responseString}"
                        };
                    }
                }

                var result = JsonConvert.DeserializeObject<ApiResponse<AuthResponseDto>>(responseString);

                if (result?.Success == true && result.Data != null)
                {
                    Console.WriteLine($"[DEBUG] Успешная авторизация для пользователя: {result.Data.Username}");

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
                Console.WriteLine($"[ERROR] Ошибка при входе: {ex.Message}");
                Console.WriteLine($"[ERROR] StackTrace: {ex.StackTrace}");

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
                
            }
        }

        public string GetAccessToken()
        {
            try
            {
                // Пробуем получить синхронно
                var task = SecureStorage.GetAsync("AuthToken");
                task.Wait(); // Ждем завершения
                return task.Result ?? string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Ошибка получения токена: {ex.Message}");
                return string.Empty;
            }
        }
        public async Task<ApiResponse<TransactionDto>> CreateTransactionAsync(CreateTransactionDto transaction)
        {
            try
            {
                Console.WriteLine($"[DEBUG] Создание транзакции: UserId={transaction.UserId}, Amount={transaction.Amount}, " +
                                 $"Type={transaction.Type}, CategoryId={transaction.CategoryId}, AccountId={transaction.AccountId}");

                var json = JsonConvert.SerializeObject(transaction);
                Console.WriteLine($"[DEBUG] Отправляемый JSON: {json}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("transactions", content);
                var responseString = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"[DEBUG] Ответ сервера: {response.StatusCode}");
                Console.WriteLine($"[DEBUG] Тело ответа: {responseString}");

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResponse<TransactionDto>
                    {
                        Success = false,
                        Message = $"Ошибка: {response.StatusCode} - {responseString}"
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
                Console.WriteLine($"[ERROR] Ошибка создания транзакции: {ex.Message}");
                Console.WriteLine($"[ERROR] StackTrace: {ex.StackTrace}");

                return new ApiResponse<TransactionDto>
                {
                    Success = false,
                    Message = $"Ошибка: {ex.Message}"
                };
            }
        }


    }
}