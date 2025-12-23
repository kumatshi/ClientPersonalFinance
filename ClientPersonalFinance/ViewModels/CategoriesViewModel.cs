using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClientPersonalFinance.ViewModels
{
    public partial class CategoriesViewModel : BaseViewModel
    {
        public CategoriesViewModel()
        {
            Title = "Категории";
        }

        [RelayCommand]
        private async Task LoadCategoriesAsync()
        {
            try
            {
                IsBusy = true;
                await Shell.Current.DisplayAlert("Информация", "Загрузка категорий - в разработке", "OK");
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