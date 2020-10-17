using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResumeApp.CustomElements
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DynamicListView : ContentView
    {
        public GridLength ItemHeight { get { return InternalGrid.RowHeight; } set { InternalGrid.RowHeight = value; } }
        public GridLength ItemWidth { get { return InternalGrid.ColumnWidth; } set { InternalGrid.ColumnWidth = value; } }

        public int Columns
        {
            get { return InternalGrid.Columns; }
            set
            {
                InternalGrid.Columns = value;
            }
        }

        public int Rows
        {
            get { return InternalGrid.Rows; }
            set
            {
                InternalGrid.Rows = value;
            }
        }

        public DynamicListView()
        {
            InitializeComponent();
        }

        public void AddView(View view)
        {
            InternalGrid.AddView(view);
        }
    }
}