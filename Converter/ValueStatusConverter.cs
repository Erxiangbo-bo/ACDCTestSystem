using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ACDCTestSystemPart1.Converter
{
    public class ValueStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (!string.IsNullOrEmpty((string)value) && !string.IsNullOrEmpty((string)parameter))
                {
                    return value.ToString().ToArray()[int.Parse((string)parameter)];
                }
                else
                {
                    return '2';
                }
            }
            catch
            {
                return '2';
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (string)value;
        }
    }
}
