using System.Windows.Forms;

namespace ResumeApp.WPF.Popups
{
    public partial class GatherTrinary : Form
    {
        public string Result = "";

        public GatherTrinary(string message, string title, string[] Buttons)
        {
            InitializeComponent();
        }
    }
}