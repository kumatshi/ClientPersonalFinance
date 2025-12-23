using ClientPersonalFinance.DTOs;
using ClientPersonalFinance.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ClientPersonalFinance.ViewModels
{
    public partial class TransactionsViewModel : BaseViewModel
    {
        private readonly ITransactionService _transactionService;
        private readonly IAccountService _accountService;
        private readonly ICategoryService _categoryService;

        [ObservableProperty]
        private ObservableCollection<TransactionDto> _transactions = new();

        [ObservableProperty]
        private FinancialSummaryDto? _summary;

        [ObservableProperty]
        private bool _hasData;

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        [ObservableProperty]
        private bool _isAddingTransaction;

        [ObservableProperty]
        private CreateTransactionDto _newTransaction = new();

        [ObservableProperty]
        private ObservableCollection<AccountDto> _accounts = new();

        [ObservableProperty]
        private ObservableCollection<CategoryDto> _categories = new();

        [ObservableProperty]
        private ObservableCollection<CategoryDto> _filteredCategories = new();

        [ObservableProperty]
        private AccountDto? _selectedAccount;

        [ObservableProperty]
        private CategoryDto? _selectedCategory;

        public int SelectedTransactionType
        {
            get => _newTransaction.Type;
            set
            {
                if (_newTransaction.Type != value)
                {
                    _newTransaction.Type = value;
                    OnPropertyChanged(nameof(SelectedTransactionType));
                    UpdateFilteredCategories();
                }
            }
        }

        public TransactionsViewModel(ITransactionService transactionService,
                                    IAccountService accountService,
                                    ICategoryService categoryService)
        {
            _transactionService = transactionService;
            _accountService = accountService;
            _categoryService = categoryService;
            Title = "Транзакции";

            // Инициализируем новую транзакцию
            NewTransaction.Date = DateTime.Now;
            NewTransaction.Type = 1; // По умолчанию расход
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
                    Transactions = new ObservableCollection<TransactionDto>(result.Data);
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
        private async Task LoadAccountsAndCategoriesAsync()
        {
            try
            {
                var accountsResult = await _accountService.GetAccountsAsync();
                if (accountsResult.Success && accountsResult.Data != null)
                {
                    Accounts = new ObservableCollection<AccountDto>(accountsResult.Data);
                }

                var categoriesResult = await _categoryService.GetCategoriesAsync();
                if (categoriesResult.Success && categoriesResult.Data != null)
                {
                    Categories = new ObservableCollection<CategoryDto>(categoriesResult.Data);
                    UpdateFilteredCategories();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки данных: {ex.Message}");
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
        private void ToggleAddTransaction()
        {
            IsAddingTransaction = !IsAddingTransaction;

            if (IsAddingTransaction)
            {
                // Сбрасываем форму при открытии
                NewTransaction = new CreateTransactionDto
                {
                    Date = DateTime.Now,
                    Type = 1, // По умолчанию расход
                    Amount = 0
                };

                // Сбрасываем выбранные значения
                SelectedAccount = null;
                SelectedCategory = null;
                SelectedTransactionType = 1;

                // Загружаем счета и категории если еще не загружены
                if (!Accounts.Any() || !Categories.Any())
                {
                    LoadAccountsAndCategoriesCommand.Execute(null);
                }
            }
        }

        [RelayCommand]
        private async Task AddTransactionAsync()
        {
            if (IsBusy)
                return;

            // Валидация
            if (NewTransaction.Amount <= 0)
            {
                await Shell.Current.DisplayAlert("Ошибка", "Сумма должна быть больше 0", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(NewTransaction.Description))
            {
                await Shell.Current.DisplayAlert("Ошибка", "Введите описание", "OK");
                return;
            }

            if (NewTransaction.CategoryId <= 0)
            {
                await Shell.Current.DisplayAlert("Ошибка", "Выберите категорию", "OK");
                return;
            }

            if (NewTransaction.AccountId <= 0)
            {
                await Shell.Current.DisplayAlert("Ошибка", "Выберите счет", "OK");
                return;
            }

            IsBusy = true;

            try
            {
                var result = await _transactionService.CreateTransactionAsync(NewTransaction);

                if (result.Success)
                {
                    await Shell.Current.DisplayAlert("Успех", "Транзакция добавлена", "OK");

                    // Скрываем форму добавления
                    IsAddingTransaction = false;

                    // Обновляем список транзакций
                    await LoadTransactionsAsync();
                    await LoadSummaryAsync();

                    // Сбрасываем форму
                    NewTransaction = new CreateTransactionDto
                    {
                        Date = DateTime.Now,
                        Type = 1
                    };

                    // Сбрасываем выбранные значения
                    SelectedAccount = null;
                    SelectedCategory = null;
                    SelectedTransactionType = 1;
                }
                else
                {
                    await Shell.Current.DisplayAlert("Ошибка", result.Message, "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Ошибка", $"Ошибка: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task DeleteTransactionAsync(TransactionDto transaction)
        {
            var confirm = await Shell.Current.DisplayAlert(
                "Удаление",
                $"Удалить транзакцию '{transaction.Description}' на сумму {transaction.Amount:C}?",
                "Да",
                "Нет");

            if (!confirm)
                return;

            try
            {
                var result = await _transactionService.DeleteTransactionAsync(transaction.Id);
                if (result.Success)
                {
                    await Shell.Current.DisplayAlert("Успех", "Транзакция удалена", "OK");
                    await LoadTransactionsAsync();
                    await LoadSummaryAsync();
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
        private async Task CancelAddTransactionAsync()
        {
            IsAddingTransaction = false;
            NewTransaction = new CreateTransactionDto
            {
                Date = DateTime.Now,
                Type = 1
            };
            SelectedAccount = null;
            SelectedCategory = null;
            SelectedTransactionType = 1;
        }

        partial void OnSelectedAccountChanged(AccountDto? value)
        {
            if (value != null)
            {
                NewTransaction.AccountId = value.Id;
            }
        }

        partial void OnSelectedCategoryChanged(CategoryDto? value)
        {
            if (value != null)
            {
                NewTransaction.CategoryId = value.Id;
            }
        }

        private void UpdateFilteredCategories()
        {
            if (Categories == null)
                return;

            if (NewTransaction.Type == 0) // Income
            {
                FilteredCategories = new ObservableCollection<CategoryDto>(
                    Categories.Where(c => c.Type == "Income").ToList());
            }
            else if (NewTransaction.Type == 1) // Expense
            {
                FilteredCategories = new ObservableCollection<CategoryDto>(
                    Categories.Where(c => c.Type == "Expense").ToList());
            }
            else
            {
                FilteredCategories = new ObservableCollection<CategoryDto>();
            }

            // Сбрасываем выбранную категорию при смене типа
            SelectedCategory = null;
        }
    }
}