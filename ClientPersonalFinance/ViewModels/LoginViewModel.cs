using ClientPersonalFinance.DTOs;
using ClientPersonalFinance.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClientPersonalFinance.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;

        [ObservableProperty]
        private string _usernameOrEmail = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _hasError;

        public LoginViewModel(IApiService apiService)
        {
            _apiService = apiService;
            Title = "Вход";
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (IsBusy)
                return;

            if (string.IsNullOrWhiteSpace(UsernameOrEmail))
            {
                ErrorMessage = "Введите имя пользователя или email";
                HasError = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Введите пароль";
                HasError = true;
                return;
            }

            IsBusy = true;
            ErrorMessage = string.Empty;
            HasError = false;

            try
            {
                var loginDto = new LoginRequestDto
                {
                    UsernameOrEmail = UsernameOrEmail,
                    Password = Password
                };

                var result = await _apiService.LoginAsync(loginDto);

                if (result.Success)
                {
                    await Shell.Current.GoToAsync("//MainPage");
                    Password = string.Empty;
                }
                else
                {
                    ErrorMessage = result.Message ?? "Неверные учетные данные";
                    HasError = true;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка: {ex.Message}";
                HasError = true;
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task TestConnectionAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            ErrorMessage = string.Empty;
            HasError = false;

            try
            {
                var result = await _apiService.TestConnectionAsync();

                await Application.Current.MainPage.DisplayAlert(
                    "Тест соединения",
                    result.Success ? "✅ Соединение с API установлено!" : $"❌ Ошибка: {result.Message}",
                    "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Ошибка",
                    $"Не удалось проверить соединение: {ex.Message}",
                    "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task GoToRegisterAsync()
        {
            await Shell.Current.GoToAsync("RegisterPage");
        }
    }
}