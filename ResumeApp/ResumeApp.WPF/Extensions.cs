namespace ResumeApp.WPF
{
    public static class Extensions
    {
        public static System.Windows.Media.Color ToColor(this Xamarin.Forms.Color color)
        {
            return System.Windows.Media.Color.FromArgb((byte)color.A, (byte)color.R, (byte)color.G, (byte)color.B);
        }
    }
}