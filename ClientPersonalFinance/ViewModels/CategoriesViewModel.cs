using ClientPersonalFinance.DTOs;
using ClientPersonalFinance.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ClientPersonalFinance.ViewModels
{
    public partial class CategoriesViewModel : BaseViewModel
    {
        private readonly ICategoryService _categoryService;

        [ObservableProperty]
        private ObservableCollection<CategoryDto> _categories = new();

        [ObservableProperty]
        private ObservableCollection<CategoryDto> _incomeCategories = new();

        [ObservableProperty]
        private ObservableCollection<CategoryDto> _expenseCategories = new();

        [ObservableProperty]
        private bool _hasData;

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        [ObservableProperty]
        private int _selectedTabIndex = 0; 

        public CategoriesViewModel(ICategoryService categoryService)
        {
            _categoryService = categoryService;
            Title = "Категории";
        }

        [RelayCommand]
        private async Task LoadCategoriesAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            StatusMessage = "Загрузка категорий...";

            try
            {
                var result = await _categoryService.GetCategoriesAsync();

                if (result.Success && result.Data != null)
                {
                    Categories = new ObservableCollection<CategoryDto>(result.Data);

                    IncomeCategories = new ObservableCollection<CategoryDto>(
                        Categories.Where(c => c.Type == "Income").ToList());

                    ExpenseCategories = new ObservableCollection<CategoryDto>(
                        Categories.Where(c => c.Type == "Expense").ToList());

                    HasData = Categories.Any();
                    StatusMessage = HasData ? $"Загружено категорий: {Categories.Count}" : "Нет категорий";
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
        private async Task DeleteCategoryAsync(CategoryDto category)
        {
            var confirm = await Shell.Current.DisplayAlert(
                "Удаление",
                $"Удалить категорию '{category.Name}'?\n\nЭто действие невозможно отменить!",
                "Да",
                "Нет");

            if (!confirm)
                return;

            try
            {
                var result = await _categoryService.DeleteCategoryAsync(category.Id);
                if (result.Success)
                {
                    await Shell.Current.DisplayAlert("Успех", "Категория удалена", "OK");
                    await LoadCategoriesAsync();
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
        private async Task AddCategoryAsync()
        {
            await Shell.Current.DisplayAlert("Информация", "Добавление категории - в разработке", "OK");
        }

        [RelayCommand]
        private async Task RefreshAsync()
        {
            await LoadCategoriesAsync();
        }
    }
}