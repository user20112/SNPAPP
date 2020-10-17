using SkiaSharp;
using System.Windows.Forms;

namespace ResumeApp.WPF.Popups
{
    public partial class Toast : Form
    {
        public Toast(SKRect size, string message)
        {
            InitializeComponent();
            ToastLabel.Text = message;
            this.Width = (int)size.Width;
            this.Height = (int)size.Height;
        }

        private void OKButton_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
    }
}