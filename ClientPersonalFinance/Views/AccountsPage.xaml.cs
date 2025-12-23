using ClientPersonalFinance.ViewModels;

namespace ClientPersonalFinance.Views
{
    public partial class AccountsPage : ContentPage
    {
        public AccountsPage(AccountsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}