using ClientPersonalFinance.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClientPersonalFinance.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;

        [ObservableProperty]
        private string _welcomeMessage = "Добро пожаловать!";

        public MainViewModel(IApiService apiService)
        {
            _apiService = apiService;
            Title = "Главная";
        }

        [RelayCommand]
        private async Task LoadUserProfileAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                var profile = await _apiService.GetUserProfileAsync();
                if (profile.Success && profile.Data != null)
                {
                    WelcomeMessage = $"Привет, {profile.Data.Username}!";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки профиля: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task LogoutAsync()
        {
            if (IsBusy)
                return;

            var confirm = await Application.Current.MainPage.DisplayAlert(
                "Подтверждение",
                "Вы уверены, что хотите выйти?",
                "Да",
                "Нет");

            if (!confirm)
                return;

            IsBusy = true;

            try
            {
                await _apiService.LogoutAsync();
                await Shell.Current.GoToAsync("//LoginPage");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Ошибка",
                    $"Не удалось выйти: {ex.Message}",
                    "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task GoToTransactionsAsync()
        {
            await Shell.Current.GoToAsync("TransactionsPage");
        }

        [RelayCommand]
        private async Task GoToAccountsAsync()
        {
            await Shell.Current.GoToAsync("AccountsPage");
        }

        [RelayCommand]
        private async Task GoToCategoriesAsync()
        {
            await Shell.Current.GoToAsync("CategoriesPage");
        }

        [RelayCommand]
        private async Task RefreshAsync()
        {
            await LoadUserProfileAsync();
        }
    }
}