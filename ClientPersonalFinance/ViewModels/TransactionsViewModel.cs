using ClientPersonalFinance.DTOs;
using ClientPersonalFinance.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClientPersonalFinance.ViewModels
{
    public partial class TransactionsViewModel : BaseViewModel
    {
        private readonly ITransactionService _transactionService;

        [ObservableProperty]
        private List<TransactionDto> _transactions = new();

        [ObservableProperty]
        private FinancialSummaryDto? _summary;

        [ObservableProperty]
        private bool _hasData;

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        public TransactionsViewModel(ITransactionService transactionService)
        {
            _transactionService = transactionService;
            Title = "Транзакции";
        }

        [RelayCommand]
        private async Task LoadTransactionsAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            StatusMessage = "Загрузка...";

            try
            {
                var result = await _transactionService.GetTransactionsAsync();

                if (result.Success && result.Data != null)
                {
                    Transactions = result.Data.ToList();
                    HasData = Transactions.Any();
                    StatusMessage = HasData ? $"Загружено: {Transactions.Count}" : "Нет транзакций";
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
        private async Task LoadSummaryAsync()
        {
            try
            {
                var result = await _transactionService.GetFinancialSummaryAsync();
                if (result.Success && result.Data != null)
                {
                    Summary = result.Data;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки сводки: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task RefreshAsync()
        {
            await LoadTransactionsAsync();
            await LoadSummaryAsync();
        }

        [RelayCommand]
        private async Task GoBackAsync()
        {
            // Простая навигация назад
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task AddTransactionAsync()
        {
            await Shell.Current.DisplayAlert("Информация", "Добавление транзакции - в разработке", "OK");
        }

        [RelayCommand]
        private async Task DeleteTransactionAsync(TransactionDto transaction)
        {
            var confirm = await Shell.Current.DisplayAlert(
                "Удаление",
                $"Удалить транзакцию '{transaction.Description}'?",
                "Да",
                "Нет");

            if (!confirm)
                return;

            try
            {
                var result = await _transactionService.DeleteTransactionAsync(transaction.Id);
                if (result.Success)
                {
                    await LoadTransactionsAsync();
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
    }
}