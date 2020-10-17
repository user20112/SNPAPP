using System;
using System.Globalization;
using System.IO;
using Xamarin.Forms;

namespace ResumeApp.Classes.Converters
{
    public class Base64DecoderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                return ImageSource.FromStream(() => new MemoryStream(System.Convert.FromBase64String((string)value)));
            return null;
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