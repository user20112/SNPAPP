using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResumeApp.CustomElements
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DynamicGrid : ContentView
    {
        private readonly List<View> Views = new List<View>();
        private bool _ExpandRight = true;
        private bool _ExpandDown = false;
        private int _Rows = 0;
        private int _Columns = 0;
        public bool ExpandRight { get { return _ExpandRight; } set { _ExpandRight = value; _ExpandDown = !value; } }
        public bool ExpandDown { get { return _ExpandDown; } set { _ExpandDown = value; _ExpandRight = !value; } }

        public int Rows
        {
            get { return _Rows; }
            set
            {
                int x = value - gridLayout.RowDefinitions.Count;
                if (x < 0)
                {
                    gridLayout.RowDefinitions.Clear();
                    x = value - Columns;
                }
                while (x > 0)
                {
                    gridLayout.RowDefinitions.Add(new RowDefinition() { Height = _RowHeight });
                    x--;
                }
                _Rows = value;
            }
        }

        public int Columns
        {
            get { return _Columns; }
            set
            {
                int x = value - Columns;
                if (x < 0)
                {
                    gridLayout.ColumnDefinitions.Clear();
                    x = value - Columns;
                }
                while (x > 0)
                {
                    gridLayout.ColumnDefinitions.Add(new ColumnDefinition() { Width = _ColumnWidth });
                    x--;
                }
                _Columns = value;
            }
        }

        private int CurrentRow = 0;
        private int CurrentColumn;
        private GridLength _RowHeight = new GridLength(1, GridUnitType.Star);

        private GridLength _ColumnWidth = new GridLength(1, GridUnitType.Star);

        public GridLength RowHeight { get { return _RowHeight; } set { _RowHeight = value; ResizeViews(); } }
        public GridLength ColumnWidth { get { return _ColumnWidth; } set { _ColumnWidth = value; ResizeViews(); } }

        public DynamicGrid()
        {
            InitializeComponent();
        }

        public void ResizeViews()
        {
            foreach (RowDefinition definition in gridLayout.RowDefinitions)
                definition.Height = RowHeight;
            foreach (ColumnDefinition definition in gridLayout.ColumnDefinitions)
                definition.Width = ColumnWidth;
        }

        public void AddView(View view)
        {
            if (Views.Count == 0)
            {
                if (Rows == 0)
                    Rows++;
                if (Columns == 0)
                    Columns++;
            }
            if (ExpandDown)
            {
                if (CurrentColumn >= Columns)
                {
                    CurrentColumn = 0;
                    CurrentRow++;
                    Rows++;
                }
                gridLayout.Children.Add(view, CurrentColumn, CurrentRow);
                CurrentColumn++;
            }
            if (ExpandRight)
            {
                if (CurrentRow >= Rows)
                {
                    CurrentRow = 0;
                    CurrentColumn++;
                    Columns++;
                }
                gridLayout.Children.Add(view, CurrentColumn, CurrentRow);
                CurrentRow++;
            }
            Views.Add(view);
        }
    }
}