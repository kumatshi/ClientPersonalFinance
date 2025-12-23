using ClientPersonalFinance.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using ClientPersonalFinance.Services;
using ClientPersonalFinance.ViewModels;
using Microsoft.Extensions.DependencyInjection;


namespace ClientPersonalFinance.Views
{
    public partial class TransactionsPage : ContentPage
    {
        public TransactionsPage()
        {
            InitializeComponent();

            // Получаем ViewModel через Dependency Injection
            var transactionService = MauiProgram.Services.GetRequiredService<ITransactionService>();
            var accountService = MauiProgram.Services.GetRequiredService<IAccountService>();
            var categoryService = MauiProgram.Services.GetRequiredService<ICategoryService>();

            BindingContext = new TransactionsViewModel(transactionService, accountService, categoryService);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is TransactionsViewModel viewModel)
            {
                viewModel.LoadTransactionsCommand?.Execute(null);
                viewModel.LoadSummaryCommand?.Execute(null);
            }
        }
    }
}