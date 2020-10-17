using ResumeApp.Classes;
using System.Collections.ObjectModel;

namespace ResumeApp.ViewModels
{
    internal class TwitterViewModel : BaseViewModel
    {
        public TwitterViewModel()
        {
            TweetsPickerSource = new ObservableCollection<string>();
        }

        public ObservableCollection<string> TweetsPickerSource { get; set; }

        public void RefreshListView()
        {
            OnPropertyChanged(nameof(TweetsPickerSource));
        }
    }
}