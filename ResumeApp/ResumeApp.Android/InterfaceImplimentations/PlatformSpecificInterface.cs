using Android;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Net.Wifi;
using Android.OS;
using Android.Support.V4.Content;
using Android.Widget;
using NativeFeatures.Droid;
using ResumeApp.Droid;
using ResumeApp.Droid.PlatformSpecific;
using ResumeApp.Interfaces;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

[assembly: Xamarin.Forms.Dependency(typeof(PlatformSpecificInterface))]

namespace NativeFeatures.Droid
{
    public class PlatformSpecificInterface : IPlatformSpecific
    {
        private SynchronizationContext Main = SynchronizationContext.Current;
        public bool SupportsLocation { get; } = true;
        private EditText input;
        private LocationListener LocationService;
        private int RequestID = 1;
        private bool ReturnBool = false;
        private string ReturnString = "";
        private string ReturnText = "";
        private bool shown = false;
        private bool toggle = true;
        private string[] TrinaryStrings = new string[1] { "" };
        public int DPI { get => (int)Application.Context.Resources.DisplayMetrics.DensityDpi; set { } }

        public int IconHeight
        {
            get
            {
                return DPI switch
                {
                    640 => 192,
                    480 => 144,
                    320 => 96,
                    240 => 72,
                    160 => 48,
                    120 => 36,
                    _ => 36,
                };
            }
            set { }
        }

        public int IconWidth
        {
            get
            {
                return DPI switch
                {
                    640 => 192,
                    480 => 144,
                    320 => 96,
                    240 => 72,
                    160 => 48,
                    120 => 36,
                    _ => 36,
                };
            }
            set { }
        }

        public bool Location
        {
            get
            {
                if (LocationService == null)
                    return false;
                return LocationService.isRequestingLocationUpdates;
            }
            set
            {
                if (value)
                {
                    if (LocationService == null)
                        LocationService = new LocationListener();
                    if (!LocationService.isRequestingLocationUpdates)
                        LocationService.StartLocationStreaming();
                }
                else
                {
                    if (LocationService != null)
                        if (LocationService.isRequestingLocationUpdates)
                            LocationService.StopLocationStreaming();
                }
            }
        }

        public string Platform { get => "Android"; set { } }
        public int ScreenHeight { get => (int)Application.Context.Resources.DisplayMetrics.HeightPixels; set { } }
        public int ScreenWidth { get => (int)Application.Context.Resources.DisplayMetrics.WidthPixels; set { } }

        public string GatherThroughPopupOkCancel(string message, string title, string HintText, string DefaultText)
        {
            shown = true;
            MainThread.BeginInvokeOnMainThread(() =>
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(MainActivity.Instance);
                builder.SetTitle(title);
                input = new EditText(Application.Context)
                {
                    Hint = HintText
                };
                if (DefaultText != "")
                {
                    input.Text = DefaultText;
                }
                input.SetTextColor(Color.Black);
                input.SetHintTextColor(Color.Gray);
                builder.SetView(input);
                builder.SetPositiveButton("Ok", OKGatherThrough);
                builder.SetNegativeButton("Cancel", CancelGatherThrough);
                builder.Show();
            });
            while (shown)
            {
                Thread.Sleep(200);
            }
            if (ReturnText == "")
                return HintText;
            return ReturnText;
        }

        public string GetAppVersion()
        {
            return Application.Context.ApplicationContext.PackageManager.GetPackageInfo(Application.Context.ApplicationContext.PackageName, 0).VersionName;
        }

        public string GetBuildVersion()
        {
            return Application.Context.ApplicationContext.PackageManager.GetPackageInfo(Application.Context.ApplicationContext.PackageName, 0).VersionCode.ToString();
        }

        public string GetDeviceVersion()
        {
            return Build.VERSION.SdkInt.ToString() + Build.VERSION.Release;
        }

        public string GetLat()
        {
            return LocationService.latitude;
        }

        public string GetLong()
        {
            return LocationService.longitude;
        }

        public string GetSSID()
        {
            WifiManager wifiManager = (WifiManager)(Application.Context.GetSystemService(Context.WifiService));
            if (wifiManager != null)
            {
                if (wifiManager.ConnectionInfo.SSID.Contains("unknown"))
                {
                    if (toggle)
                    {
                        RequestLocationPermission();
                        toggle = false;
                        return GetSSID();
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return wifiManager.ConnectionInfo.SSID;
                }
            }
            else
            {
                return "";
            }
        }

        public bool IsBluetoothOn()
        {
            BluetoothAdapter mBluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            if (mBluetoothAdapter == null)
            {
                return false;
            }
            else if (!mBluetoothAdapter.IsEnabled)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool IsLocationOn()
        {
            return ContextCompat.CheckSelfPermission(MainActivity.Instance, Manifest.Permission.AccessFineLocation) == Permission.Granted;
        }

        public void PlacePhoneCall(string number)
        {
            try
            {
                PhoneDialer.Open(number);
            }
            catch (ArgumentNullException)
            {
                // Number was null or white space
            }
            catch (FeatureNotSupportedException)
            {
                // Phone Dialer is not supported on this device.
            }
            catch (Exception)
            {
                // Other error has occurred.
            }
        }

        public bool PopupBool(string message, string title, string PositiveButtonText, string NegativeButtonText)
        {
            shown = true;
            MainThread.BeginInvokeOnMainThread(() =>
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(MainActivity.Instance);
                builder.SetTitle(title);
                builder.SetMessage(message);
                builder.SetPositiveButton(PositiveButtonText, PopupBoolPositive);
                builder.SetNegativeButton(NegativeButtonText, PopupBoolNegative);
                builder.Show();
            });
            while (shown)
            {
                Thread.Sleep(200);
            }
            return ReturnBool;
        }

        public string PopupTrinary(string message, string title, string[] Buttons)
        {
            shown = true;
            TrinaryStrings = Buttons;
            MainThread.BeginInvokeOnMainThread(() =>
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(MainActivity.Instance);
                builder.SetTitle(title);
                builder.SetMessage(message);
                builder.SetPositiveButton(Buttons[0], PopupTrinaryPos);
                builder.SetNeutralButton(Buttons[1], PopupTrinaryNeu);
                builder.SetNegativeButton(Buttons[2], PopupTrinaryNeg);
                builder.Show();
            });
            while (shown)
            {
                Thread.Sleep(200);
            }
            return ReturnString;
        }

        public void RequestBluetoothPermission()
        {
            Task.Run(() =>
            {
                MainActivity.Instance.RequestPermissions(new string[1] { Manifest.Permission.BluetoothAdmin }, Main);
            });
        }

        public void RequestLocationPermission()
        {
            Task.Run(() =>
            {
                MainActivity.Instance.RequestPermissions(new string[2] { Manifest.Permission.AccessFineLocation, Manifest.Permission.AccessCoarseLocation }, Main);
            });
        }

        public byte[] ResizeImage(byte[] imageData, float width, float height)
        {
            // Load the bitmap
            Bitmap originalImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
            var Height = originalImage.Height;
            var Width = originalImage.Width;
            float Temp;
            float temp;
            if (Height > Width)
            {
                Temp = height;
                float teiler = Height / height;
                temp = Width / teiler;
            }
            else
            {
                temp = width;
                float teiler = Width / width;
                Temp = Height / teiler;
            }//scale the loaded bitmap up.
            Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)temp, (int)Temp, false);
            using MemoryStream ms = new MemoryStream();
            resizedImage.Compress(Bitmap.CompressFormat.Png, 100, ms);
            return ms.ToArray();
        }

        public void SendTextEmail(string Topic, string data, string recipient)
        {
            try
            {
                var message = new EmailMessage
                {
                    Subject = Topic,
                    Body = data,
                    To = new List<string>(new string[1] { recipient })
                };
                Email.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException)
            {
                // Email is not supported on this device
            }
            catch (Exception)
            {
                // Some other exception occurred
            }
        }

        public void ShareFile(string path)
        {
            Share.RequestAsync(new ShareFileRequest(new ShareFile(path)));
        }

        public void ShowToast(SKRect rectangleToDraw, string message)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Toast.MakeText(Application.Context, message, ToastLength.Long).Show();
            });
        }

        public byte[] SKImageToByte(SKImage Image)
        {
            using Bitmap Bitmap = SkiaSharp.Views.Android.AndroidExtensions.ToBitmap(SKBitmap.FromImage(Image));
            using var stream = new MemoryStream();
            Bitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);
            byte[] data = stream.ToArray();
            return data;
        }

        private void CancelGatherThrough(object sender, DialogClickEventArgs e)
        {
            ReturnText = "";
            shown = false;
        }

        private void ExistingProject(object sender, DialogClickEventArgs e)
        {
        }

        private void OKGatherThrough(object sender, DialogClickEventArgs e)
        {
            ReturnText = input.Text;
            shown = false;
        }

        private void PopupBoolNegative(object sender, DialogClickEventArgs e)
        {
            ReturnBool = false;
            shown = false;
        }

        private void PopupBoolPositive(object sender, DialogClickEventArgs e)
        {
            ReturnBool = true;
            shown = false;
        }

        private void PopupTrinaryNeg(object sender, DialogClickEventArgs e)
        {
            ReturnString = TrinaryStrings[2];
            shown = false;
        }

        private void PopupTrinaryNeu(object sender, DialogClickEventArgs e)
        {
            ReturnString = TrinaryStrings[1];
            shown = false;
        }

        private void PopupTrinaryPos(object sender, DialogClickEventArgs e)
        {
            ReturnString = TrinaryStrings[0];
            shown = false;
        }

        public bool CheckCameraPermission()
        {
            return ContextCompat.CheckSelfPermission(MainActivity.Instance, Manifest.Permission.Camera) == Permission.Granted;
        }

        public void RequestCameraPermission()
        {
            Task.Run(() =>
            {
                MainActivity.Instance.RequestPermissions(new string[1] { Manifest.Permission.Camera }, Main);
            });
        }
    }
}