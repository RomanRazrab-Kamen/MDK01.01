using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace PR16.Services
{
    public class PriceColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                double price = 0;

                if (value is decimal decPrice)
                    price = (double)decPrice;
                else if (value is double dblPrice)
                    price = dblPrice;
                else if (value is float fltPrice)
                    price = (double)fltPrice;
                else if (value is int intPrice)
                    price = intPrice;
                else
                    double.TryParse(value.ToString().Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out price);

                if (price > 5000)
                    return new SolidColorBrush(Colors.LightCoral);

                if (price < 3000)
                    return new SolidColorBrush(Colors.LightGreen);
            }

            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
