using ClientPersonalFinance.ViewModels;

namespace ClientPersonalFinance.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
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