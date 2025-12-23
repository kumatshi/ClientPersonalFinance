namespace ClientPersonalFinance;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Создаем Shell как главную страницу
        MainPage = new AppShell();
    }
}