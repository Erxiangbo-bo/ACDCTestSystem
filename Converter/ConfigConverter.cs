using ACDCTestSystemPart1.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ACDCTestSystemPart1.Converter
{
    public class ConfigConverter : IValueConverter
    {
        public ConnectionConfig Config
        {
            get; set;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Config = new ConnectionConfig();
            try
            {
                if (value != null && parameter != null)
                {
                    if (value.GetType() != Config.GetType())
                    {
                        return null;
                    }
                    Config = value as ConnectionConfig;
                    var obj = value.GetType().GetProperties();
                    foreach (var each in obj)
                    {
                        if (each.Name.Equals((string)parameter))
                        {
                            return each.GetValue(Config);
                        }
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var obj = typeof(ConnectionConfig).GetProperties();
                foreach (var each in obj)
                {
                    if (each.Name.Equals((string)parameter))
                    {
                        each.SetValue(Config, (string)value);
                        return Config;
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
