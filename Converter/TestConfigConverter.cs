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
    public class TestConfigConverter : IValueConverter
    {
        public TestConfig TestConfig
        {
            get; set;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TestConfig = new TestConfig();
            try
            {
                if (value != null && parameter != null)
                {
                    if (value.GetType() != TestConfig.GetType())
                    {
                        return null;
                    }
                    TestConfig = value as TestConfig;
                    var obj = value.GetType().GetProperties();
                    foreach (var each in obj)
                    {
                        if (each.Name.Equals((string)parameter))
                        {
                            return each.GetValue(TestConfig);
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
        {//数据类型问题----未解决
            try
            {
                var obj = typeof(TestConfig).GetProperties();
                foreach (var each in obj)
                {
                    if (each.Name.Equals((string)parameter))
                    {
                        each.SetValue(TestConfig, (string)value);
                        return TestConfig;
                    }
                }
                return null;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                return null;
            }
        }
    }
}
