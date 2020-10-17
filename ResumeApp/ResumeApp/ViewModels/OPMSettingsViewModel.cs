using ResumeApp.Classes;
using System.Collections.ObjectModel;

namespace ResumeApp.ViewModels
{
    public class OPMSettingsViewModel : BaseViewModel
    {
        public OPMSettingsViewModel()
        {
            ColorCodePickerSource = new ObservableCollection<string>();
        }

        public ObservableCollection<string> ColorCodePickerSource { get; set; }

        public void RefreshListView()
        {
            OnPropertyChanged(nameof(ColorCodePickerSource));
        }
    }
}