using ResumeApp.Classes;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;

namespace ResumeApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class USBPage : BaseContentPage
    {
        public USBPage()
        {
            InitializeComponent();
            Title = "USB";
        }

        public override void OnFirstAppearing()
        {
            base.OnFirstAppearing();
            HIDDeviceInterface.DevicePath = null;
            NavigateTo(new USBSelectPage(this));
        }

        public override void OnReAppearing()
        {
            base.OnReAppearing();
            if (!HIDDeviceInterface.Connected)
                NavigateBack();
        }

        internal void SetDevice(string DeviceID)
        {
            HIDDeviceInterface.Load(DeviceID);
            Task.Run(() =>
            {
                try
                {
                    while (!OnPage)
                        Thread.Sleep(50);// wait till we navigate back to page
                    Main.Post(delegate { DisplayLabel.Text = "Connected Attempting First Read"; }, null);
                    Task.Run(() =>
                    {
                        Thread.Sleep(1000);
                        Main.Post(delegate
                        {
                            if (DisplayLabel.Text == "Connected Attempting First Read")
                            {
                                DisplayLabel.Text = "Could not read from device.";
                            }
                        }, null);
                    });
                    byte[] Buffer = HIDDeviceInterface.Read();
                    Main.Post(delegate
                    {
                        DisplayLabel.Text = "Connected and Streaming";
                        ReceivedLabel.Text = ASCIIEncoding.ASCII.GetString(Buffer);
                    }, null);
                    while (OnPage)// only while we are still on the page.
                    {
                        Buffer = HIDDeviceInterface.Read();
                        Main.Post(delegate { ReceivedLabel.Text = ASCIIEncoding.ASCII.GetString(Buffer); }, null);
                        Thread.Sleep(100);
                    }
                }
                catch
                {
                    Main.Post(delegate { DisplayLabel.Text = "Permission Denied For USB Device."; }, null);
                }
            });
        }

        private void SendButton_Clicked(object sender, System.EventArgs e)
        {
        }
    }
}