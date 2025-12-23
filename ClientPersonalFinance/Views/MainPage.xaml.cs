using ClientPersonalFinance.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace ClientPersonalFinance.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            // Получаем ViewModel через Dependency Injection
            var viewModel = MauiProgram.Services.GetRequiredService<MainViewModel>();
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is MainViewModel viewModel)
            {
                viewModel.LoadUserProfileCommand?.Execute(null);
            }
        }
    }
}