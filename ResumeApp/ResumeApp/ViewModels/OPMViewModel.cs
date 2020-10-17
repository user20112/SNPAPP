using ResumeApp.Classes;
using ResumeApp.Classes.Adapters;
using System.Collections.ObjectModel;

namespace ResumeApp.ViewModels
{
    public class OPMViewModel : BaseViewModel
    {
        public OPMViewModel()
        {
            ReadingListSource = new ObservableCollection<ReadingListAdapter>();
        }

        public ObservableCollection<ReadingListAdapter> ReadingListSource { get; set; }

        public void RefreshListView()
        {
            OnPropertyChanged("ReadingListSource");
        }
    }
}