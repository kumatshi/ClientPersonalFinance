using ClientPersonalFinance.Services;
using ClientPersonalFinance.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace ClientPersonalFinance.Views
{
    public partial class AccountsPage : ContentPage
    {
        public AccountsPage()
        {
            InitializeComponent();
            var accountService = MauiProgram.Services.GetRequiredService<IAccountService>();
            BindingContext = new AccountsViewModel(accountService);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}