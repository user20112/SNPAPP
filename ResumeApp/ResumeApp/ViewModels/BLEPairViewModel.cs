using ResumeApp.Classes;
using System.Collections.ObjectModel;

namespace ResumeApp.ViewModels
{
    internal class BLEPairViewModel : BaseViewModel
    {
        public BLEPairViewModel()
        {
            DetectedDevicesList = new ObservableCollection<string>();
        }

        public ObservableCollection<string> DetectedDevicesList { get; set; }
    }
}