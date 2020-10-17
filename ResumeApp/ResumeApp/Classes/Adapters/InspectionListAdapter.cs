using ResumeApp.DataClasses;
using System.ComponentModel;
using Xamarin.Forms;

namespace ResumeApp.Classes.Adapters
{
    public class InspectionListAdapter : INotifyPropertyChanged
    {
        public Color _highlightedColor;
        private readonly Color DefualtColor;
        private bool _selected;

        public InspectionListAdapter(InspectionSaveData inspection)
        {
            Selected = false;
            Inspection = inspection;
            DefualtColor = (Color)Application.Current.Resources["DefualtColor"];
            _highlightedColor = (Color)Application.Current.Resources["HighLightColor"];
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Color HighlightColor
        {
            get
            {
                if (Selected)
                {
                    return _highlightedColor;
                }
                else
                {
                    return DefualtColor;
                }
            }
            set
            {
                _highlightedColor = value;
            }
        }

        public InspectionSaveData Inspection { get; set; }
        public bool Selected { get { return _selected; } set { if (_selected != value) { _selected = value; OnPropertyChanged(nameof(HighlightColor)); } } }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}