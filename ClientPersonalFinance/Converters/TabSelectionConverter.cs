using System.Globalization;

namespace ClientPersonalFinance.Converters
{
    public class TabSelectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int selectedIndex && parameter is string paramString)
            {
                var parts = paramString.Split('|');
                if (parts.Length >= 2)
                {
                    if (int.TryParse(parts[0], out int tabIndex))
                    {
                        return selectedIndex == tabIndex ?
                               parts[1] :
                               (parts.Length > 2 ? parts[2] : "#E0E0E0");
                    }
                }
            }
            return "#E0E0E0"; 
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}