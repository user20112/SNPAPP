using ResumeApp.Interfaces;
using ResumeApp.WPF.InterfaceImplimentations;
using ResumeApp.WPF.Popups;
using ResumeApp.WPF.SSID;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Windows.Devices.Geolocation;
using Windows.Devices.Radios;
using Windows.Foundation;

[assembly: Xamarin.Forms.Dependency(typeof(PlatformSpecificInterface))]

namespace ResumeApp.WPF.InterfaceImplimentations
{
    public class PlatformSpecificInterface : IPlatformSpecific
    {
        public int DPI { get { return (int)PresentationSource.FromVisual(MainWindow.Instance).CompositionTarget.TransformToDevice.M11 * 96; } set { } }
        public int IconHeight { get => 80; set { } }
        public int IconWidth { get => 80; set { } }
        private bool _location = false;
        public bool Location { get => _location; set => _location = value; }
        public string Platform { get => "Windows"; set { } }
        public int ScreenHeight { get => (int)MainWindow.Instance.ActualHeight; set { } }
        public int ScreenWidth { get => (int)MainWindow.Instance.ActualWidth; set { } }

        public bool SupportsLocation { get; } = false;

        public string GatherThroughPopupOkCancel(string message, string title, string HintText, string DefaultText)
        {
            GatherOKCancel popup = new GatherOKCancel(message, title, HintText, DefaultText);
            popup.ShowDialog();
            return popup.Result;
        }

        public string GetAppVersion()
        {
            return App.ResourceAssembly.GetName().Version.ToString();
        }

        public string GetBuildVersion()
        {
            return "1";
        }

        public string GetDeviceVersion()
        {
            OperatingSystem os = Environment.OSVersion;
            Version vs = os.Version;
            string operatingSystem = "";
            if (os.Platform == PlatformID.Win32Windows)
            {
                switch (vs.Minor)
                {
                    case 0:
                        operatingSystem = "95";
                        break;

                    case 10:
                        if (vs.Revision.ToString() == "2222A")
                            operatingSystem = "98SE";
                        else
                            operatingSystem = "98";
                        break;

                    case 90:
                        operatingSystem = "Me";
                        break;

                    default:
                        break;
                }
            }
            else if (os.Platform == PlatformID.Win32NT)
            {
                switch (vs.Major)
                {
                    case 3:
                        operatingSystem = "NT 3.51";
                        break;

                    case 4:
                        operatingSystem = "NT 4.0";
                        break;

                    case 5:
                        if (vs.Minor == 0)
                            operatingSystem = "2000";
                        else
                            operatingSystem = "XP";
                        break;

                    case 6:
                        if (vs.Minor == 0)
                            operatingSystem = "Vista";
                        else if (vs.Minor == 1)
                            operatingSystem = "7";
                        else if (vs.Minor == 2)
                            operatingSystem = "8";
                        else
                            operatingSystem = "8.1";
                        break;

                    case 10:
                        operatingSystem = "10";
                        break;

                    default:
                        break;
                }
            }
            if (operatingSystem != "")
            {
                operatingSystem = "Windows " + operatingSystem;
                if (os.ServicePack != "")
                {
                    operatingSystem += " " + os.ServicePack;
                }
            }
            return operatingSystem;
        }

        public string GetLat()
        {
            throw new System.NotImplementedException();
        }

        public string GetLong()
        {
            throw new System.NotImplementedException();
        }

        public string GetSSID()
        {
            IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            if (nics != null)
            {
                if (nics.Length > 0)
                {
                    List<Network> networks = new List<Network>(NetworkListManager.GetNetworks(NetworkConnectivityLevels.Connected));
                    foreach (Network network in networks)
                    {//could support returning all but interface only allows returning one so return first.
                        return network.Description;
                    }
                }
            }
            return "";
        }

        public bool IsBluetoothOn()
        {
            IAsyncOperation<IReadOnlyList<Radio>> temp = Radio.GetRadiosAsync();
            while (temp.Status != AsyncStatus.Completed && temp.Status != AsyncStatus.Error)
            {
                Thread.Sleep(50);
            }
            IReadOnlyList<Radio> radios = temp.GetResults();
            Radio bluetoothRadio = radios.FirstOrDefault(radio => radio.Kind == RadioKind.Bluetooth);
            return bluetoothRadio != null && bluetoothRadio.State == RadioState.On;
        }

        public bool IsLocationOn()
        {
            IAsyncOperation<GeolocationAccessStatus> temp = Geolocator.RequestAccessAsync();
            while (temp.Status != AsyncStatus.Completed && temp.Status != AsyncStatus.Error)
            {
                Thread.Sleep(50);
            }
            GeolocationAccessStatus accessStatus = temp.GetResults();
            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:
                    return true;

                case GeolocationAccessStatus.Denied:
                    return false;

                case GeolocationAccessStatus.Unspecified:
                    return false;
            }
            return false;
        }

        public void PlacePhoneCall(string number)
        {
            throw new System.NotImplementedException();
        }

        public bool PopupBool(string message, string title, string PositiveButtonText, string NegativeButtonText)
        {
            GatherBool popup = new GatherBool(message, title, PositiveButtonText, NegativeButtonText);
            popup.ShowDialog();
            return popup.Result;
        }

        public string PopupTrinary(string message, string title, string[] Buttons)
        {
            GatherTrinary popup = new GatherTrinary(message, title, Buttons);
            popup.ShowDialog();
            return popup.Result;
        }

        public void RequestBluetoothPermission()
        {
            //throw new System.NotImplementedException();
        }

        public void RequestLocationPermission()
        {
            //throw new System.NotImplementedException();
        }

        public byte[] ResizeImage(byte[] imageData, float width, float height)
        {
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(CreateResizedImage((Bitmap)Bitmap.FromStream(new MemoryStream(imageData)), (int)width, (int)height, 0));
            using (MemoryStream stream = new MemoryStream())
            {
                encoder.Save(stream);
                return stream.ToArray();
            }
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                return bitmapimage;
            }
        }

        private BitmapFrame CreateResizedImage(Bitmap source, int width, int height, int margin)
        {
            var rect = new Rect(margin, margin, width - margin * 2, height - margin * 2);
            var group = new DrawingGroup();
            RenderOptions.SetBitmapScalingMode(group, BitmapScalingMode.HighQuality);
            group.Children.Add(new ImageDrawing(BitmapToImageSource(source), rect));
            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
                drawingContext.DrawDrawing(group);
            var resizedImage = new RenderTargetBitmap(
                width, height,
                96, 96,
                PixelFormats.Default);
            resizedImage.Render(drawingVisual);
            return BitmapFrame.Create(resizedImage);
        }

        public void SendTextEmail(string Topic, string data, string recipient)
        {
            Process.Start("mailto:" + recipient + "?subject=" + Topic + "&body=" + data);
        }

        public void ShareFile(string path)
        {
            ShareFilePopup ShareFile = new ShareFilePopup(path);
            ShareFile.ShowDialog();
        }

        public static void CloseModalWindows()
        {
            // get the main window
            AutomationElement root = AutomationElement.FromHandle(Process.GetCurrentProcess().MainWindowHandle);
            if (root == null)
                return;
            // it should implement the Window pattern
            if (!root.TryGetCurrentPattern(WindowPattern.Pattern, out object pattern))
                return;
            WindowPattern window = (WindowPattern)pattern;
            if (window.Current.WindowInteractionState != WindowInteractionState.ReadyForUserInteraction)
            {
                // get sub windows
                foreach (AutomationElement element in root.FindAll(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window)))
                {
                    // hmmm... is it really a window?
                    if (element.TryGetCurrentPattern(WindowPattern.Pattern, out pattern))
                    {
                        // if it's ready, try to close it
                        WindowPattern childWindow = (WindowPattern)pattern;
                        if (childWindow.Current.WindowInteractionState == WindowInteractionState.ReadyForUserInteraction)
                        {
                            childWindow.Close();
                        }
                    }
                }
            }
        }

        public void ShowToast(SKRect rectangleToDraw, string message)
        {
            CloseModalWindows();
            Toast toast = new Toast(rectangleToDraw, message);
            toast.ShowDialog();
        }

        public byte[] SKImageToByte(SKImage Image)
        {
            return Image.Encode().ToArray();
        }

        public bool CheckCameraPermission()
        {
            throw new NotImplementedException();
        }

        public void RequestCameraPermission()
        {
            throw new NotImplementedException();
        }
    }
}