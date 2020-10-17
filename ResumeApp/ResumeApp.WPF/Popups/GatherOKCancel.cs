using System.Windows.Forms;

namespace ResumeApp.WPF.Popups
{
    public partial class GatherOKCancel : Form
    {
        public string Result = "";

        public GatherOKCancel(string message, string title, string HintText, string DefaultText)
        {
            InitializeComponent();
        }
    }
}