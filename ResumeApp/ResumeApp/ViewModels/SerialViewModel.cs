using ResumeApp.Classes;
using System.Collections.ObjectModel;

namespace ResumeApp.ViewModels
{
    public class SerialViewModel : BaseViewModel
    {
        public SerialViewModel()
        {
            SerialDevicePickerSource = new ObservableCollection<string>();
            StopBitsPickerSource = new ObservableCollection<string>();
            ParityPickerSource = new ObservableCollection<string>();
            BaudRatePickerSource = new ObservableCollection<string>();
            DataBitsPickerSource = new ObservableCollection<string>();
        }

        public ObservableCollection<string> SerialDevicePickerSource { get; set; }
        public ObservableCollection<string> StopBitsPickerSource { get; set; }
        public ObservableCollection<string> ParityPickerSource { get; set; }
        public ObservableCollection<string> BaudRatePickerSource { get; set; }
        public ObservableCollection<string> DataBitsPickerSource { get; set; }

        public void RefreshListView()
        {
            OnPropertyChanged(nameof(SerialDevicePickerSource));
            OnPropertyChanged(nameof(StopBitsPickerSource));
            OnPropertyChanged(nameof(ParityPickerSource));
            OnPropertyChanged(nameof(BaudRatePickerSource));
            OnPropertyChanged(nameof(DataBitsPickerSource));
        }
    }
}