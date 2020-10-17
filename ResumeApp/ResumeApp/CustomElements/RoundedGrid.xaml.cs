using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Xamarin.Forms.Grid;

namespace ResumeApp.CustomElements
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoundedGrid : ContentView
    {
        public IList<IGestureRecognizer> OnTap { get { return OuterFrame.GestureRecognizers; } }

        public float CornerRadius
        {
            get { return OuterFrame.CornerRadius; }
            set { OuterFrame.CornerRadius = value; }
        }

        public new Color BackgroundColor
        {
            get { return OuterFrame.BackgroundColor; }
            set { OuterFrame.BackgroundColor = value; }
        }

        public ColumnDefinitionCollection ColumnDefinitions
        {
            get { return InnerGrid.ColumnDefinitions; }
            set { InnerGrid.ColumnDefinitions = value; }
        }

        public RowDefinitionCollection RowDefinitions
        {
            get { return InnerGrid.RowDefinitions; }
            set { InnerGrid.RowDefinitions = value; }
        }

        public new IGridList<View> Children
        {
            get { return InnerGrid.Children; }
        }

        public RoundedGrid()
        {
            InitializeComponent();
        }
    }
}