using ClientPersonalFinance.ViewModels;

namespace ClientPersonalFinance.Views
{
    public partial class TransactionsPage : ContentPage
    {
        public TransactionsPage(TransactionsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
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