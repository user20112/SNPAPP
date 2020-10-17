using ResumeApp.Interfaces;
using ResumeApp.UWP.InterfaceImplimentations;
using ResumeApp.UWP.PlatformSpecific;
using SkiaSharp;
using SkiaSharp.Views.UWP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Calls;
using Windows.Devices.Geolocation;
using Windows.Devices.Radios;
using Windows.Devices.WiFi;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.System.Profile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Xamarin.Essentials;

[assembly: Xamarin.Forms.Dependency(typeof(PlatformSpecificInterface))]

namespace ResumeApp.UWP.InterfaceImplimentations
{
    internal class PlatformSpecificInterface : IPlatformSpecific
    {
        private const double ToastDelay = 3.5;
        private bool _Location = false;
        private Geoposition CurrentPositionData;
        private WiFiAdapter FirstAdapter;

        private Geolocator LocationService;
        public bool SupportsLocation => true;

        public int DPI { get { return (int)DisplayInformation.GetForCurrentView().LogicalDpi; } set => throw new NotImplementedException(); }
        public int IconHeight { get; set; } = 100;
        public int IconWidth { get; set; } = 100;

        public bool Location
        {
            get
            {
                return _Location;
            }
            set
            {
                if (value && !_Location)
                    StartLocation();
                else if (!value)
                    StopLocation();
                _Location = value;
            }
        }

        public string Platform { get { return "Windows"; } set => throw new NotImplementedException(); }

        public int ScreenHeight { get { return (int)DisplayInformation.GetForCurrentView().ScreenHeightInRawPixels; } set => throw new NotImplementedException(); }

        public int ScreenWidth { get { return (int)DisplayInformation.GetForCurrentView().ScreenWidthInRawPixels; } set => throw new NotImplementedException(); }

        public bool CheckCameraPermission()
        {
            return true;
        }

        public string GatherThroughPopupOkCancel(string message, string title, string HintText, string DefaultText)
        {
            IAsyncOperation<ContentDialogResult> task = null;
            GetStringPopup dialog1 = null;
            RunOnUISynchronous(() =>
            {
                dialog1 = new GetStringPopup(message, title, HintText, DefaultText);
                task = dialog1.ShowAsync();
            });
            while (task.Status == AsyncStatus.Started)
            {
                Thread.Sleep(50);
            }
            ContentDialogResult result = task.GetResults();
            string temp = "";
            RunOnUISynchronous(() =>
            {
                temp = dialog1.Text;
            });
            return temp;
        }

        public string GetAppVersion()
        {
            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;
            return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
        }

        public string GetBuildVersion()
        {
            return "1";
        }

        public string GetDeviceVersion()
        {
            string deviceFamilyVersion = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
            ulong version = ulong.Parse(deviceFamilyVersion);
            ulong major = (version & 0xFFFF000000000000L) >> 48;
            ulong minor = (version & 0x0000FFFF00000000L) >> 32;
            ulong build = (version & 0x00000000FFFF0000L) >> 16;
            ulong revision = (version & 0x000000000000FFFFL);
            return $"{major}.{minor}.{build}.{revision}";
        }

        public string GetLat()
        {
            if (CurrentPositionData != null)
                return CurrentPositionData.Coordinate.Point.Position.Latitude.ToString("0.000");
            else
                return "";
        }

        public string GetLong()
        {
            if (CurrentPositionData != null)
                return CurrentPositionData.Coordinate.Point.Position.Longitude.ToString("0.000");
            else
                return "";
        }

        public string GetSSID()
        {
            if (FirstAdapter.NetworkAdapter.GetConnectedProfileAsync() != null)
            {
                Task<Windows.Networking.Connectivity.ConnectionProfile> ConnectedProfile = FirstAdapter.NetworkAdapter.GetConnectedProfileAsync().AsTask<Windows.Networking.Connectivity.ConnectionProfile>();
                ConnectedProfile.Wait();
                Windows.Networking.Connectivity.ConnectionProfile connectedProfile = ConnectedProfile.Result;
                if (connectedProfile != null)
                    return connectedProfile.ProfileName;
            }
            return "";
        }

        public bool IsBluetoothOn()
        {
            IAsyncOperation<IReadOnlyList<Radio>> Task = Radio.GetRadiosAsync();
            while (Task.Status == AsyncStatus.Started)
            {
                Thread.Sleep(50);
            }
            IReadOnlyList<Radio> radios = Task.GetResults();
            var bluetoothRadio = radios.FirstOrDefault(radio => radio.Kind == RadioKind.Bluetooth);
            return bluetoothRadio != null && bluetoothRadio.State == RadioState.On;
        }

        public bool IsLocationOn()
        {
            IAsyncOperation<GeolocationAccessStatus> Task = Geolocator.RequestAccessAsync();
            while (Task.Status == AsyncStatus.Started)
            {
                Thread.Sleep(50);
            }
            return Task.GetResults() == GeolocationAccessStatus.Allowed;
        }

        public void PlacePhoneCall(string number)
        {
            PhoneCallManager.ShowPhoneCallUI(number, "");
        }

        public bool PopupBool(string message, string title, string PositiveButtonText, string NegativeButtonText)
        {
            IAsyncOperation<ContentDialogResult> task = null;
            RunOnUISynchronous(() =>
            {
                ContentDialog locationPromptDialog = new ContentDialog
                {
                    Title = title,
                    Content = message,
                    CloseButtonText = NegativeButtonText,
                    PrimaryButtonText = PositiveButtonText
                };
                task = locationPromptDialog.ShowAsync();
            });
            while (task.Status == AsyncStatus.Started)
            {
                Thread.Sleep(50);
            }
            ContentDialogResult result = task.GetResults();
            return result == ContentDialogResult.Primary;
        }

        public string PopupTrinary(string message, string title, string[] Buttons)
        {
            ContentDialog locationPromptDialog = new ContentDialog
            {
                Title = title,
                Content = message,
                CloseButtonText = Buttons[0],
                PrimaryButtonText = Buttons[1],
                SecondaryButtonText = Buttons[2]
            };
            IAsyncOperation<ContentDialogResult> task = locationPromptDialog.ShowAsync();
            while (task.Status == AsyncStatus.Started)
            {
                Thread.Sleep(50);
            }
            ContentDialogResult result = task.GetResults();
            switch (result)
            {
                case ContentDialogResult.None: return Buttons[0];
                case ContentDialogResult.Primary: return Buttons[1];
                case ContentDialogResult.Secondary: return Buttons[2];
                default:
                    break;
            }
            return Buttons[0];
        }

        public void RequestBluetoothPermission()
        {
            //throw new NotImplementedException();
        }

        public void RequestCameraPermission()
        {
        }

        public void RequestLocationPermission()
        {
            IAsyncOperation<GeolocationAccessStatus> Task = Geolocator.RequestAccessAsync();
            while (Task.Status == AsyncStatus.Started)
                Thread.Sleep(50);
        }

        public byte[] ResizeImage(byte[] imageData, float width, float height)
        {
            return imageData;
        }

        public void RunOnUISynchronous(Action action)
        {
            bool Completed = false;
            MainThread.BeginInvokeOnMainThread(() =>
            {
                action();
                Completed = true;
            });
            while (!Completed)
                Thread.Sleep(50);
        }

        public void SendTextEmail(string Topic, string data, string recipient)
        {
            try
            {
                Windows.ApplicationModel.Email.EmailMessage emailMessage = new Windows.ApplicationModel.Email.EmailMessage
                {
                    Body = data
                };
                Windows.ApplicationModel.Contacts.ContactEmail email = new Windows.ApplicationModel.Contacts.ContactEmail() { Address = recipient };
                if (email != null)
                {
                    var emailRecipient = new Windows.ApplicationModel.Email.EmailRecipient(email.Address);
                    emailMessage.To.Add(emailRecipient);
                    emailMessage.Subject = Topic;
                }
                _ = Windows.ApplicationModel.Email.EmailManager.ShowComposeNewEmailAsync(emailMessage);
            }
            catch
            {
            }
        }

        public void ShareFile(string path)
        {
            Share.RequestAsync(new ShareFileRequest
            {
                Title = "SNP File",
                File = new ShareFile(path)
            });
        }

        public void ShowToast(SKRect rectangleToDraw, string message)
        {
            TextBlock label = new TextBlock
            {
                Text = message,
                Foreground = new SolidColorBrush(Windows.UI.Colors.White),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            Style style = new Style { TargetType = typeof(FlyoutPresenter) };
            style.Setters.Add(new Setter(Control.BackgroundProperty, new SolidColorBrush(Windows.UI.Colors.Black)));
            style.Setters.Add(new Setter(FrameworkElement.MaxHeightProperty, 1));
            Flyout toast = new Flyout
            {
                Content = label,
                Placement = FlyoutPlacementMode.Full,
                FlyoutPresenterStyle = style,
            };
            toast.ShowAt(Window.Current.Content as FrameworkElement);
            DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(ToastDelay) };
            timer.Tick += (sender, e) =>
            {
                timer.Stop();
                toast.Hide();
            };
            timer.Start();
        }

        public byte[] SKImageToByte(SKImage Image)
        {
            return Image.ToWriteableBitmap().PixelBuffer.ToArray();
        }

        private void OnStatusChanged(Geolocator sender, StatusChangedEventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                switch (e.Status)
                {
                    case PositionStatus.Ready:
                        //If its ready to get location data,you can add some code.
                        IAsyncOperation<Geoposition> task = LocationService.GetGeopositionAsync();
                        while (task.Status == AsyncStatus.Started)
                            Thread.Sleep(50);
                        CurrentPositionData = task.GetResults();
                        break;

                    case PositionStatus.Initializing:
                        //Location is being initialized.waiting for it to complete.
                        break;

                    case PositionStatus.NoData:
                        //Some places can not access location.Metros,Mountains,Elevators or fields with jammers.This case works when you're in one of them.
                        break;

                    case PositionStatus.Disabled:
                        //You either rejected location access at start or closed Location.
                        break;

                    case PositionStatus.NotInitialized:
                        //The app has not yet accessed location data.
                        break;

                    case PositionStatus.NotAvailable:
                        //Location may not be possible due to OS settings.
                        break;

                    default:
                        //If non of above works,this will.Writing a message helps.
                        break;
                }
            });
        }

        private void StartLocation()
        {
            IAsyncOperation<GeolocationAccessStatus> task = Geolocator.RequestAccessAsync();
            Task.Run(() =>
            {
                while (task.Status == Windows.Foundation.AsyncStatus.Started)
                    Thread.Sleep(50);
                GeolocationAccessStatus access = task.GetResults();
                switch (access)
                {
                    case GeolocationAccessStatus.Allowed:
                        LocationService = new Geolocator();
                        LocationService.StatusChanged += OnStatusChanged;
                        break;

                    case GeolocationAccessStatus.Denied:
                        //You can show a message here if user declined.
                        break;
                }
            });
        }

        private void StopLocation()
        {
        }
    }
}