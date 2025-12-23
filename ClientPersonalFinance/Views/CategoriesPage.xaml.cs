using ClientPersonalFinance.Services;
using ClientPersonalFinance.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace ClientPersonalFinance.Views
{
    public partial class CategoriesPage : ContentPage
    {
        public CategoriesPage()
        {
            InitializeComponent();
            var categoryService = MauiProgram.Services.GetRequiredService<ICategoryService>();
            BindingContext = new CategoriesViewModel(categoryService);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}