using System;
using System.Globalization;
using Xamarin.Forms;

namespace ResumeApp.Classes.Converters
{
    public class DropDownSymbolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool Value = (bool)value;
            if (Value)
            {
                return "V";
            }
            else
            {
                return ">";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Only one way bindings are supported with this converter");
        }

        public object ProvideValue()
        {
            return this;
        }
    }
}