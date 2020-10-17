using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ResumeApp.UWP.PlatformSpecific
{
    public sealed partial class GetStringPopup : ContentDialog
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(GetStringPopup), new PropertyMetadata(default(string)));
        private string DefaultText = "";

        public GetStringPopup(string message, string title, string HintText, string DefaultText)
        {
            this.DefaultText = DefaultText;
            InitializeComponent();
            InputTextBox.PlaceholderText = HintText;
            Title = title;
            PrimaryButtonText = "OK";
            SecondaryButtonText = "Cancel";
            MessageLabel.Text = message;
        }

        public string Text { get { return (string)GetValue(TextProperty); } set { SetValue(TextProperty, value); } }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (InputTextBox.Text == "")
                Text = DefaultText;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Text = "";
        }
    }
}