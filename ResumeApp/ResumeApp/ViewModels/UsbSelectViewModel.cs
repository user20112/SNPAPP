using ResumeApp.Classes;
using System.Collections.ObjectModel;

namespace ResumeApp.ViewModels
{
    public class UsbSelectViewModel : BaseViewModel
    {
        public UsbSelectViewModel()
        {
            USBDevices = new ObservableCollection<string>();
        }

        public ObservableCollection<string> USBDevices { get; set; }
    }
}