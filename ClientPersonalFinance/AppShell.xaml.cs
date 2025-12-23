using ClientPersonalFinance.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ClientPersonalFinance;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Регистрация маршрутов
        Routing.RegisterRoute("LoginPage", typeof(Views.LoginPage));
        Routing.RegisterRoute("RegisterPage", typeof(Views.RegisterPage));
        Routing.RegisterRoute("MainPage", typeof(Views.MainPage));
        Routing.RegisterRoute("TransactionsPage", typeof(Views.TransactionsPage));
        Routing.RegisterRoute("AccountsPage", typeof(Views.AccountsPage));
        Routing.RegisterRoute("CategoriesPage", typeof(Views.CategoriesPage));
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CheckAuthenticationAsync();
    }

    private async Task CheckAuthenticationAsync()
    {
        try
        {
            var apiService = App.Current?.Handler?.MauiContext?.Services?.GetService<IApiService>();

            if (apiService == null)
            {
                await GoToAsync("//LoginPage");
                return;
            }

            var isAuthenticated = await apiService.IsAuthenticatedAsync();

            if (isAuthenticated)
            {
                await GoToAsync("//MainPage");
            }
            else
            {
                await GoToAsync("//LoginPage");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] {ex.Message}");
            await GoToAsync("//LoginPage");
        }
    }
}