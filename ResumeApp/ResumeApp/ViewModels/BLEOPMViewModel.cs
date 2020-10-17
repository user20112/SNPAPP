using ResumeApp.Classes;
using System.Collections.ObjectModel;

namespace ResumeApp.ViewModels
{
    internal class BLEOPMViewModel : BaseViewModel
    {
        public BLEOPMViewModel()
        {
            DetectedDevicesList = new ObservableCollection<string>();
        }

        public ObservableCollection<string> DetectedDevicesList { get; set; }

        public void RefreshListView()
        {
            OnPropertyChanged("ColorCodeListSource");
        }
    }
}