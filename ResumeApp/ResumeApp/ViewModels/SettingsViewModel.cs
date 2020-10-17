using ResumeApp.Classes;
using System.Collections.ObjectModel;

namespace ResumeApp.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public SettingsViewModel()
        {
            ThemePickerSource = new ObservableCollection<string>();
        }

        public ObservableCollection<string> ThemePickerSource { get; set; }

        public void RefreshPage()
        {
            OnPropertyChanged(nameof(ThemePickerSource));
        }
    }
}