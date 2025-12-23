using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClientPersonalFinance.ViewModels
{
    public partial class AccountsViewModel : BaseViewModel
    {
        public AccountsViewModel()
        {
            Title = "Мои счета";
        }

        [RelayCommand]
        private async Task LoadAccountsAsync()
        {
            try
            {
                IsBusy = true;
                await Shell.Current.DisplayAlert("Информация", "Загрузка счетов - в разработке", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Ошибка", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task GoBackAsync()
        {
            // Простая навигация назад
            await Shell.Current.GoToAsync("..");
        }
    }
}