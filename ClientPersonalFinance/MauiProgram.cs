using ClientPersonalFinance.Converters;
using ClientPersonalFinance.Services;
using ClientPersonalFinance.ViewModels;
using ClientPersonalFinance.Views;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace ClientPersonalFinance;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .UseMauiCommunityToolkit();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Загружаем стили
        builder.ConfigureMauiHandlers(handlers => { });

        // Регистрация конвертеров
        builder.Services.AddSingleton<BalanceColorConverter>();
        builder.Services.AddSingleton<TransactionTypeColorConverter>();
        builder.Services.AddSingleton<NotNullConverter>();
        builder.Services.AddSingleton<InverseBooleanConverter>();

        // Регистрация сервисов
        builder.Services.AddSingleton<IApiService, ApiService>();
        builder.Services.AddSingleton<ITransactionService, TransactionService>();
        builder.Services.AddSingleton<IAccountService, AccountService>();
        builder.Services.AddSingleton<ICategoryService, CategoryService>();

        // Регистрация ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<TransactionsViewModel>();
        builder.Services.AddTransient<AccountsViewModel>();
        builder.Services.AddTransient<CategoriesViewModel>();

        // Регистрация Views
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<TransactionsPage>();
        builder.Services.AddTransient<AccountsPage>();
        builder.Services.AddTransient<CategoriesPage>();

        return builder.Build();
    }
}