using ResumeApp.Classes;
using System.Collections.ObjectModel;

namespace ResumeApp.ViewModels
{
    internal class HueViewModel : BaseViewModel
    {
        public HueViewModel()
        {
            LightsPickerSource = new ObservableCollection<string>();
        }

        public ObservableCollection<string> LightsPickerSource { get; set; }

        public void RefreshListView()
        {
            OnPropertyChanged(nameof(LightsPickerSource));
        }
    }
}