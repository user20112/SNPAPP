using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResumeApp.CustomElements
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HorizontalListView : ContentView
    {
        public GridLength ItemWidth { get { return InternalGrid.ColumnWidth; } set { InternalGrid.ColumnWidth = value; InternalGrid.ResizeViews(); } }
        public GridLength ItemHeight { get { return InternalGrid.RowHeight; } set { InternalGrid.RowHeight = value; InternalGrid.ResizeViews(); } }

        public HorizontalListView()
        {
            InitializeComponent();
        }

        public void AddView(View view)
        {
            InternalGrid.AddView(view);
        }
    }
}