using ClientPersonalFinance.DTOs;
using ClientPersonalFinance.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ClientPersonalFinance.ViewModels
{
    public partial class AccountsViewModel : BaseViewModel
    {
        private readonly IAccountService _accountService;

        [ObservableProperty]
        private ObservableCollection<AccountDto> _accounts = new();

        [ObservableProperty]
        private bool _hasData;

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        [ObservableProperty]
        private decimal _totalBalance;

        public AccountsViewModel(IAccountService accountService)
        {
            _accountService = accountService;
            Title = "Мои счета";
        }

        [RelayCommand]
        private async Task LoadAccountsAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            StatusMessage = "Загрузка счетов...";

            try
            {
                var result = await _accountService.GetAccountsAsync();

                if (result.Success && result.Data != null)
                {
                    Accounts = new ObservableCollection<AccountDto>(result.Data);
                    HasData = Accounts.Any();
                    TotalBalance = Accounts.Sum(a => a.Balance);
                    StatusMessage = HasData ? $"Загружено счетов: {Accounts.Count}" : "Нет счетов";
                }
                else
                {
                    StatusMessage = result.Message;
                    await Shell.Current.DisplayAlert("Ошибка", result.Message, "OK");
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
                await Shell.Current.DisplayAlert("Ошибка", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task DeleteAccountAsync(AccountDto account)
        {
            var confirm = await Shell.Current.DisplayAlert(
                "Удаление",
                $"Удалить счет '{account.Name}' с балансом {account.Balance:C}?\n\nЭто действие невозможно отменить!",
                "Да",
                "Нет");

            if (!confirm)
                return;

            try
            {
                var result = await _accountService.DeleteAccountAsync(account.Id);
                if (result.Success)
                {
                    await Shell.Current.DisplayAlert("Успех", "Счет удален", "OK");
                    await LoadAccountsAsync();
                }
                else
                {
                    await Shell.Current.DisplayAlert("Ошибка", result.Message, "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Ошибка", ex.Message, "OK");
            }
        }

        [RelayCommand]
        private async Task GoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task AddAccountAsync()
        {
            await Shell.Current.DisplayAlert("Информация", "Добавление счета - в разработке", "OK");
        }

        [RelayCommand]
        private async Task RefreshAsync()
        {
            await LoadAccountsAsync();
        }
    }
}