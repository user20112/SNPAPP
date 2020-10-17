using System;
using System.Globalization;
using Xamarin.Forms;

namespace ResumeApp.Classes.Converters
{
    public class IncrementIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((int)value) + 1;
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