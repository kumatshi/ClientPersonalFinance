using ClientPersonalFinance.ViewModels;

namespace ClientPersonalFinance.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage(LoginViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is LoginViewModel viewModel)
            {
                viewModel.Password = string.Empty;
                viewModel.ErrorMessage = string.Empty;
                viewModel.HasError = false;
            }
        }
    }
}