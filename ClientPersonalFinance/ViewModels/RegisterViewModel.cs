using ClientPersonalFinance.DTOs;
using ClientPersonalFinance.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClientPersonalFinance.ViewModels
{
    public partial class RegisterViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;

        [ObservableProperty]
        private string _username = string.Empty;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _confirmPassword = string.Empty;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _hasError;

        public RegisterViewModel(IApiService apiService)
        {
            _apiService = apiService;
            Title = "Регистрация";
        }

        [RelayCommand]
        private async Task RegisterAsync()
        {
            if (IsBusy)
                return;

            if (string.IsNullOrWhiteSpace(Username) || Username.Length < 3)
            {
                ErrorMessage = "Имя пользователя должно быть не менее 3 символов";
                HasError = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(Email) || !IsValidEmail(Email))
            {
                ErrorMessage = "Введите корректный email";
                HasError = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(Password) || Password.Length < 6)
            {
                ErrorMessage = "Пароль должен быть не менее 6 символов";
                HasError = true;
                return;
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Пароли не совпадают";
                HasError = true;
                return;
            }

            IsBusy = true;
            ErrorMessage = string.Empty;
            HasError = false;

            try
            {
                var registerDto = new RegisterRequestDto
                {
                    Username = Username,
                    Email = Email,
                    Password = Password,
                    ConfirmPassword = ConfirmPassword
                };

                var result = await _apiService.RegisterAsync(registerDto);

                if (result.Success)
                {
                    await Shell.Current.GoToAsync("//MainPage");
                    ClearForm();
                }
                else
                {
                    ErrorMessage = result.Message;
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
        private async Task GoToLoginAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        private void ClearForm()
        {
            Username = string.Empty;
            Email = string.Empty;
            Password = string.Empty;
            ConfirmPassword = string.Empty;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}