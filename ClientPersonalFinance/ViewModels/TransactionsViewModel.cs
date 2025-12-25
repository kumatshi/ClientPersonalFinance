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

        private CreateTransactionDto _newTransaction = new();
        public CreateTransactionDto NewTransaction
        {
            get => _newTransaction;
            set => SetProperty(ref _newTransaction, value);
        }

        [ObservableProperty]
        private ObservableCollection<AccountDto> _accounts = new();

        [ObservableProperty]
        private ObservableCollection<CategoryDto> _categories = new();

        [ObservableProperty]
        private ObservableCollection<CategoryDto> _filteredCategories = new();

        private AccountDto? _selectedAccount;
        public AccountDto? SelectedAccount
        {
            get => _selectedAccount;
            set
            {
                if (SetProperty(ref _selectedAccount, value) && value != null)
                {
                    NewTransaction.AccountId = value.Id;
                }
            }
        }

        private CategoryDto? _selectedCategory;
        public CategoryDto? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (SetProperty(ref _selectedCategory, value) && value != null)
                {
                    NewTransaction.CategoryId = value.Id;
                }
            }
        }

        private int _selectedTransactionType = 1;
        public int SelectedTransactionType
        {
            get => _selectedTransactionType;
            set
            {
                if (SetProperty(ref _selectedTransactionType, value))
                {
                    NewTransaction.Type = value;
                    UpdateFilteredCategories();
                }
            }
        }
        public decimal TransactionAmount
        {
            get => NewTransaction.Amount;
            set
            {
                if (NewTransaction.Amount != value)
                {
                    NewTransaction.Amount = value;
                    OnPropertyChanged(nameof(TransactionAmount));
                }
            }
        }

        public string TransactionDescription
        {
            get => NewTransaction.Description;
            set
            {
                if (NewTransaction.Description != value)
                {
                    NewTransaction.Description = value;
                    OnPropertyChanged(nameof(TransactionDescription));
                }
            }
        }

        public DateTime TransactionDate
        {
            get => NewTransaction.Date;
            set
            {
                if (NewTransaction.Date != value)
                {
                    NewTransaction.Date = value;
                    OnPropertyChanged(nameof(TransactionDate));
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

            NewTransaction.Date = DateTime.Now;
            NewTransaction.Type = 1; 
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
                ResetForm();


                if (!Accounts.Any() || !Categories.Any())
                {
                    LoadAccountsAndCategoriesCommand.Execute(null);
                }
            }
        }

        [RelayCommand]
        private async Task AddTransactionAsync()
        {
            Console.WriteLine("[DEBUG] AddTransactionAsync вызван");

            if (IsBusy)
            {
                Console.WriteLine("[DEBUG] IsBusy = true, команда не выполняется");
                return;
            }

            Console.WriteLine("[DEBUG] Проверка валидации данных...");

            if (NewTransaction.Amount <= 0)
            {
                Console.WriteLine($"[DEBUG] Ошибка: Amount = {NewTransaction.Amount}");
                await Shell.Current.DisplayAlert("Ошибка", "Сумма должна быть больше 0", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(NewTransaction.Description))
            {
                Console.WriteLine("[DEBUG] Ошибка: Description пустое");
                await Shell.Current.DisplayAlert("Ошибка", "Введите описание", "OK");
                return;
            }

            if (NewTransaction.CategoryId <= 0)
            {
                Console.WriteLine($"[DEBUG] Ошибка: CategoryId = {NewTransaction.CategoryId}");
                await Shell.Current.DisplayAlert("Ошибка", "Выберите категорию", "OK");
                return;
            }

            if (NewTransaction.AccountId <= 0)
            {
                Console.WriteLine($"[DEBUG] Ошибка: AccountId = {NewTransaction.AccountId}");
                await Shell.Current.DisplayAlert("Ошибка", "Выберите счет", "OK");
                return;
            }

            // Получаем UserId
            var userIdString = await SecureStorage.GetAsync("UserId");
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                Console.WriteLine("[DEBUG] Ошибка: UserId не найден в SecureStorage");
                await Shell.Current.DisplayAlert("Ошибка", "Пользователь не авторизован. Войдите заново.", "OK");
                return;
            }

            NewTransaction.UserId = userId;
            Console.WriteLine($"[DEBUG] UserId установлен: {userId}");

            IsBusy = true;
            Console.WriteLine($"[DEBUG] IsBusy установлен в true");

            try
            {
                Console.WriteLine($"[DEBUG] Отправка транзакции:");
                Console.WriteLine($"  Amount: {NewTransaction.Amount}");
                Console.WriteLine($"  Description: {NewTransaction.Description}");
                Console.WriteLine($"  Type: {NewTransaction.Type}");
                Console.WriteLine($"  CategoryId: {NewTransaction.CategoryId}");
                Console.WriteLine($"  AccountId: {NewTransaction.AccountId}");
                Console.WriteLine($"  UserId: {NewTransaction.UserId}");
                Console.WriteLine($"  Date: {NewTransaction.Date}");

                var result = await _transactionService.CreateTransactionAsync(NewTransaction);

                if (result.Success)
                {
                    Console.WriteLine("[DEBUG] Транзакция успешно создана");
                    await Shell.Current.DisplayAlert("Успех", "Транзакция добавлена", "OK");

                    IsAddingTransaction = false;
                    await LoadTransactionsAsync();
                    await LoadSummaryAsync();
                    ResetForm();
                }
                else
                {
                    Console.WriteLine($"[DEBUG] Ошибка от сервера: {result.Message}");
                    await Shell.Current.DisplayAlert("Ошибка", result.Message, "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Исключение: {ex.Message}");
                Console.WriteLine($"[ERROR] StackTrace: {ex.StackTrace}");
                await Shell.Current.DisplayAlert("Ошибка", $"Ошибка: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
                Console.WriteLine($"[DEBUG] IsBusy установлен в false");
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
        private void CancelAddTransactionAsync()
        {
            IsAddingTransaction = false;
            ResetForm();
        }

        private void ResetForm()
        {
            NewTransaction = new CreateTransactionDto
            {
                Date = DateTime.Now,
                Type = 1, // Expense по умолчанию
                Amount = 0,
                Description = string.Empty,
                CategoryId = 0,
                AccountId = 0,
                UserId = 0 // Устанавливаем 0, будет установлено при создании
            };
            SelectedAccount = null;
            SelectedCategory = null;
            SelectedTransactionType = 1;

            OnPropertyChanged(nameof(TransactionAmount));
            OnPropertyChanged(nameof(TransactionDescription));
            OnPropertyChanged(nameof(TransactionDate));
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
            SelectedCategory = null;
        }
    }
}