using SkiaSharp;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace ResumeApp.Classes.Converters
{
    public class SKColorColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SKColor Value = (SKColor)value;
            double Red = Scale(Value.Red, 0, 1);
            double Blue = Scale(Value.Blue, 0, 1);
            double Green = Scale(Value.Green, 0, 1);
            double Alpha = Scale(Value.Alpha, 0, 1);
            return new Color(Red, Green, Blue, Alpha);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Only one way bindings are supported with this converter");
        }

        public object ProvideValue()
        {
            return this;
        }

        public double Scale(double val, double min, double max)
        {
            double m = (max - min) / (255);
            double c = min;
            return val * m + c;
        }
    }
}