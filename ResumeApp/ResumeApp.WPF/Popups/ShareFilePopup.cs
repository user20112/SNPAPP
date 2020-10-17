using System.Windows.Forms;

namespace ResumeApp.WPF.Popups
{
    public partial class ShareFilePopup : Form
    {
        private string FileToShare;

        public ShareFilePopup(string FileToShare)
        {
            InitializeComponent();
            this.FileToShare = FileToShare;
        }
    }
}