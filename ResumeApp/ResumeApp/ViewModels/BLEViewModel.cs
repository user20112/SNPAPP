using ResumeApp.Classes;
using ResumeApp.Classes.Adapters;
using System.Collections.ObjectModel;

namespace ResumeApp.ViewModels
{
    public class BLEViewModel : BaseViewModel
    {
        public BLEViewModel()
        {
            ServicesListSource = new ObservableCollection<ServiceListViewAdapter>();
        }

        public ObservableCollection<ServiceListViewAdapter> ServicesListSource { get; set; }

        public void RefreshListView()
        {
            OnPropertyChanged("Source");
        }
    }
}