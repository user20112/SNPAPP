using System.Windows.Forms;

namespace ResumeApp.WPF.Popups
{
    public partial class GatherBool : Form
    {
        public bool Result = false;

        public GatherBool(string message, string title, string PositiveButtonText, string NegativeButtonText)
        {
            InitializeComponent();
        }
    }
}