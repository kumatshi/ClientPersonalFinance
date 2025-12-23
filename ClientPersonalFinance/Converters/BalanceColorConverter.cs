using System.Globalization;

namespace ClientPersonalFinance.Converters
{
    public class BalanceColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal balance)
            {
                return balance >= 0 ? Color.FromArgb("#4CAF50") : Color.FromArgb("#F44336");
            }
            return Color.FromArgb("#757575"); // Серый по умолчанию
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}