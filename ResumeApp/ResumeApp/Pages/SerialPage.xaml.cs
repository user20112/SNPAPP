using ResumeApp.Classes;
using ResumeApp.Interfaces;
using ResumeApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;

namespace ResumeApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SerialPage : BaseContentPage
    {
        private bool AlreadyLooking = false;
        private List<string> KnownDevices = new List<string>();
        private SerialViewModel vm;

        public SerialPage()
        {
            InitializeComponent();
            Title = "Serial";
            Title = "OPM Settings";
            vm = (SerialViewModel)BindingContext;
            vm.BaudRatePickerSource.Add("110");
            vm.BaudRatePickerSource.Add("300");
            vm.BaudRatePickerSource.Add("600");
            vm.BaudRatePickerSource.Add("1200");
            vm.BaudRatePickerSource.Add("2400");
            vm.BaudRatePickerSource.Add("4800");
            vm.BaudRatePickerSource.Add("9600");
            vm.BaudRatePickerSource.Add("14400");
            vm.BaudRatePickerSource.Add("19200");
            vm.BaudRatePickerSource.Add("38400");
            vm.BaudRatePickerSource.Add("57600");
            vm.BaudRatePickerSource.Add("115200");
            vm.BaudRatePickerSource.Add("230400");
            vm.BaudRatePickerSource.Add("460800");
            vm.BaudRatePickerSource.Add("921600");
            vm.DataBitsPickerSource.Add("7");
            vm.DataBitsPickerSource.Add("8");
            vm.ParityPickerSource.Add("None");
            vm.ParityPickerSource.Add("Odd");
            vm.ParityPickerSource.Add("Even");
            vm.ParityPickerSource.Add("Mark");
            vm.ParityPickerSource.Add("Space");
            vm.StopBitsPickerSource.Add("1");
            vm.StopBitsPickerSource.Add("1.5");
            vm.StopBitsPickerSource.Add("2");
            BaudRatePicker.SelectedIndex = 0;
            StopBitsPicker.SelectedIndex = 0;
            ParityPicker.SelectedIndex = 0;
            DataBitsPicker.SelectedIndex = 0;
        }

        public override void OnFirstAppearing()
        {
            base.OnFirstAppearing();
            PlatformSpecificSerialManager.DataReceived += SerialDataReceived;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Task.Run(() =>
            {
                SerialPortDetectLoop();
            });
        }

        public override void OnBackButton()
        {
            base.OnBackButton();
            PlatformSpecificSerialManager.DataReceived -= SerialDataReceived;
        }

        private void SerialDataReceived(object sender, EventArgs e)
        {
            ReadInEditor.Text += (sender as ISerialCom).ReadExisting();
        }

        public void SerialPortDetectLoop()
        {
            if (!AlreadyLooking)
            {
                AlreadyLooking = true;
                while (OnPage)
                {
                    List<string> Detected = PlatformSpecificSerialManager.Scan();
                    if (Detected.Count < KnownDevices.Count)
                        foreach (string x in KnownDevices)
                            if (!Detected.Contains(x))
                                RemovedPort(x);
                    if (Detected.Count != KnownDevices.Count)
                        foreach (string x in Detected)
                            if (!KnownDevices.Contains(x))
                                NewPort(x);
                }
                AlreadyLooking = false;
            }
        }

        private void RemovedPort(string x)
        {
            KnownDevices.Remove(x);
            Main.Post(delegate { vm.SerialDevicePickerSource.Remove(x); }, null);
        }

        private void NewPort(string x)
        {
            KnownDevices.Add(x);
            Main.Post(delegate { vm.SerialDevicePickerSource.Add(x); }, null);
        }

        private void OpenPortButton_Clicked(object sender, System.EventArgs e)
        {
            if (SerialDevicePicker.SelectedItem != null)
            {
                if (PlatformSpecificSerialManager.IsOpen)
                    PlatformSpecificSerialManager.Close();
                PlatformSpecificSerialManager.Load((string)SerialDevicePicker.SelectedItem);
                PlatformSpecificSerialManager.SetBaud(Convert.ToInt32((string)BaudRatePicker.SelectedItem));
                PlatformSpecificSerialManager.SetData(Convert.ToInt32((string)DataBitsPicker.SelectedItem));
                PlatformSpecificSerialManager.SetParity((string)ParityPicker.SelectedItem);
                PlatformSpecificSerialManager.SetStop(Convert.ToDouble((string)StopBitsPicker.SelectedItem));
                SendEditor.Text = "";
                ReadInEditor.Text = "";
                SendEditor.IsEnabled = true;
                SendButton.IsEnabled = true;
            }
        }

        private void SendButton_Clicked(object sender, EventArgs e)
        {
            string ToSend = SendEditor.Text;
            SendEditor.Text = "";
            PlatformSpecificSerialManager.Write(ToSend);
        }
    }
}