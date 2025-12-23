using ClientPersonalFinance.Views;

namespace ClientPersonalFinance;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        RegisterRoutes();
    }

    private void RegisterRoutes()
    {
        Routing.RegisterRoute("LoginPage", typeof(LoginPage));
        Routing.RegisterRoute("RegisterPage", typeof(RegisterPage));
        Routing.RegisterRoute("MainPage", typeof(MainPage));
        Routing.RegisterRoute("TransactionsPage", typeof(TransactionsPage));
        Routing.RegisterRoute("AccountsPage", typeof(AccountsPage));
        Routing.RegisterRoute("CategoriesPage", typeof(CategoriesPage));
    }
}