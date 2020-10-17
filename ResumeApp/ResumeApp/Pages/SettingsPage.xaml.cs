using ResumeApp.Classes;
using ResumeApp.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResumeApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : BaseContentPage
    {
        private readonly SettingsViewModel VM;

        public SettingsPage()
        {
            InitializeComponent();
            Title = "Settings";
            UISettingsFrame.Content = UISettingsGrid;
            VM = BindingContext as SettingsViewModel;
            VM.ThemePickerSource.Add("Blue");
            VM.ThemePickerSource.Add("Orange");
            VM.ThemePickerSource.Add("GrayScale");
            VM.ThemePickerSource.Add("Green");
            VM.ThemePickerSource.Add("Neon");
            VM.ThemePickerSource.Add("Red");
            int index = VM.ThemePickerSource.IndexOf(SelectedTheme);
            if (index >= 0)
                ThemePicker.SelectedIndex = index;
            else
                ThemePicker.SelectedIndex = 0;
            ThemePicker.SelectedIndexChanged += ThemeChanged;
        }

        private void ThemeChanged(object sender, EventArgs e)
        {
            SettingsManagerInterface.SetString((ThemePicker.SelectedItem as string), "SelectedString");
            switch ((ThemePicker.SelectedItem as string))
            {
                case "Blue":
                    Application.Current.Resources["TraceColor"] = (Color)Application.Current.Resources["LightBlue"];
                    Application.Current.Resources["NavigationPrimary"] = (Color)Application.Current.Resources["Blue"];
                    break;

                case "Orange":
                    Application.Current.Resources["TraceColor"] = (Color)Application.Current.Resources["Orange"];
                    Application.Current.Resources["NavigationPrimary"] = (Color)Application.Current.Resources["DarkOrange"];
                    break;

                case "GrayScale":
                    Application.Current.Resources["TraceColor"] = (Color)Application.Current.Resources["Black"];
                    Application.Current.Resources["NavigationPrimary"] = (Color)Application.Current.Resources["Black"];
                    break;

                case "Neon":
                    Application.Current.Resources["TraceColor"] = (Color)Application.Current.Resources["Pink"];
                    Application.Current.Resources["NavigationPrimary"] = (Color)Application.Current.Resources["DarkPink"];
                    break;

                case "Green":
                    Application.Current.Resources["TraceColor"] = (Color)Application.Current.Resources["Green"];
                    Application.Current.Resources["NavigationPrimary"] = (Color)Application.Current.Resources["DarkGreen"];
                    break;

                case "Red":
                    Application.Current.Resources["TraceColor"] = (Color)Application.Current.Resources["Red"];
                    Application.Current.Resources["NavigationPrimary"] = (Color)Application.Current.Resources["DarkRed"];
                    break;
            }
        }
    }
}