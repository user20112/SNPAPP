using ResumeApp.Classes;
using System.Collections.ObjectModel;

namespace ResumeApp.ViewModels
{
    public class AboutViewModel : BaseContentPage
    {
        public AboutViewModel()
        {
            AboutListSource = new ObservableCollection<HomePageItem>();
        }

        public ObservableCollection<HomePageItem> AboutListSource { get; set; }

        public void RefreshListView()
        {
            OnPropertyChanged("Source");
        }
    }
}