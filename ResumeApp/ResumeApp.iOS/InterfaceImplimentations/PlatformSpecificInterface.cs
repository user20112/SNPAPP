using CoreBluetooth;
using CoreGraphics;
using CoreLocation;
using Foundation;
using MessageUI;
using NativeFeatures.IOS;
using ResumeApp.Interfaces;
using ResumeApp.iOS;
using SkiaSharp;
using SkiaSharp.Views.iOS;
using System;
using System.Drawing;
using System.IO;
using System.Threading;
using SystemConfiguration;
using UIKit;
using Xamarin.Essentials;

[assembly: Xamarin.Forms.Dependency(typeof(PlatformSpecificInterface))]

namespace NativeFeatures.IOS
{
    public class PlatformSpecificInterface : IPlatformSpecific
    {
        public bool SupportsLocation { get; } = true;
        public static LocationManager Manager = new LocationManager();
        private readonly CBCentralManager CBCentralManager = new CBCentralManager();
        private CLLocation location;
        private MFMailComposeViewController mailController = null;
        private UIDocumentInteractionController shareController = null;
        private bool WaitingForComplete = true;

        ~PlatformSpecificInterface()
        {
            if (Location)
            {
                Manager.StopLocationUpdates();
                Manager.LocationUpdated -= HandleLocationChanged;
            }
        }

        public int DPI { get => (int)DeviceDisplay.MainDisplayInfo.Density; set { } }
        public int IconHeight { get => 40; set { } }
        public int IconWidth { get => 40; set { } }

        public bool Location
        {
            get { return !(location == null); }
            set
            {
                if (value)
                {
                    location = new CLLocation();
                    Manager.StartLocationUpdates();
                    Manager.LocationUpdated += HandleLocationChanged;
                }
                else
                {
                    location = null;
                    Manager.StopLocationUpdates();
                    Manager.LocationUpdated -= HandleLocationChanged;
                }
            }
        }

        public string Platform { get => "IOS"; set { } }
        public int ScreenHeight { get => (int)DeviceDisplay.MainDisplayInfo.Height; set { } }
        public int ScreenWidth { get => (int)DeviceDisplay.MainDisplayInfo.Width; set { } }

        public static UIImage ImageFromByteArray(byte[] data)
        {
            if (data == null)
            {
                return null;
            }
            UIImage image;
            try
            {
                image = new UIImage(NSData.FromArray(data));
            }
            catch
            {
                return null;
            }
            return image;
        }

        public string GatherThroughPopupOkCancel(string message, string title, string HintText, string StartingText)
        {
            UIAlertController okCancelAlertController = null;
            UIViewController vc = null;
            string Temp = "";
            UIApplication.SharedApplication.InvokeOnMainThread(() =>
            {
                UIWindow window = UIApplication.SharedApplication.KeyWindow;
                vc = window.RootViewController;
                while (vc.PresentedViewController != null)
                {
                    vc = vc.PresentedViewController;
                }
                okCancelAlertController = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
                UITextField textField = null;
                okCancelAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, alert =>
                {
                    Temp = textField.Text;
                    WaitingForComplete = false;
                }));
                okCancelAlertController.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, alert =>
                {
                    WaitingForComplete = false;
                }));
                okCancelAlertController.AddTextField((UITextField obj) =>
                {
                    obj.Placeholder = HintText;
                    if (StartingText != "")
                        obj.Text = StartingText;
                    textField = obj;
                    Temp = "";
                });
                vc.InvokeOnMainThread(() =>
                {
                    try
                    {
                        vc.PresentViewController(okCancelAlertController, true, CompletionFunction);
                    }
                    catch (Exception)
                    {
                    }
                });
            });
            while (WaitingForComplete)
            {
                Thread.Sleep(200);
            }
            WaitingForComplete = true;
            if (Temp == "")
                return HintText;
            return Temp;
        }

        public string GetAppVersion()
        {
            return NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleVersion").ToString();
        }

        public string GetBuildVersion()
        {
            return NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleShortVersionString").ToString();
        }

        public string GetDeviceVersion()
        {
            return UIDevice.CurrentDevice.SystemVersion;
        }

        public string GetLat()
        {
            if (Location)
            {
                return location.Coordinate.Latitude.ToString("0.00000");
            }
            return "Location Unavailable";
        }

        public string GetLong()
        {
            if (Location)
            {
                return location.Coordinate.Longitude.ToString("0.00000");
            }
            return "Location Unavailable";
        }

        public string GetSSID()
        {
            GetLocationConsent();
            String ssid = "";
            try
            {
                StatusCode status;
                if ((status = CaptiveNetwork.TryGetSupportedInterfaces(out string[] supportedInterfaces)) != StatusCode.OK)
                {
                }
                else
                {
                    foreach (var item in supportedInterfaces)
                    {
                        status = CaptiveNetwork.TryCopyCurrentNetworkInfo(item, out NSDictionary info);
                        if (status != StatusCode.OK)
                        {
                            continue;
                        }
                        ssid = info[CaptiveNetwork.NetworkInfoKeySSID].ToString();
                        info.Dispose();
                    }
                }
            }
            catch
            {
            }
            return ssid;
        }

        public void HandleLocationChanged(object sender, LocationUpdatedEventArgs e)
        {
            Console.WriteLine(e.Location);
            location = e.Location;
        }

        public bool IsBluetoothOn()
        {
            return CBCentralManager.State == CBCentralManagerState.PoweredOn;
        }

        public bool IsLocationOn()
        {
            return CLLocationManager.LocationServicesEnabled;
        }

        public void PlacePhoneCall(string number)
        {
            try
            {
                PhoneDialer.Open(number);
            }
            catch
            {
            }
        }

        public bool PopupBool(string message, string title, string PositiveButtonText, string NegativeButtonText)
        {
            UIAlertController okCancelAlertController = null;
            UIViewController vc = null;
            bool Temp = false;
            UIApplication.SharedApplication.InvokeOnMainThread(() =>
            {
                UIWindow window = UIApplication.SharedApplication.KeyWindow;
                vc = window.RootViewController;
                while (vc.PresentedViewController != null)
                {
                    vc = vc.PresentedViewController;
                }
                okCancelAlertController = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
                okCancelAlertController.AddAction(UIAlertAction.Create(PositiveButtonText, UIAlertActionStyle.Default, alert =>
                {
                    Temp = true;
                    WaitingForComplete = false;
                }));
                okCancelAlertController.AddAction(UIAlertAction.Create(NegativeButtonText, UIAlertActionStyle.Cancel, alert =>
                {
                    Temp = false;
                    WaitingForComplete = false;
                }));
                vc.InvokeOnMainThread(() =>
                {
                    try
                    {
                        vc.PresentViewController(okCancelAlertController, true, CompletionFunction);
                    }
                    catch (Exception)
                    {
                    }
                });
            });
            while (WaitingForComplete)
            {
                Thread.Sleep(200);
            }
            WaitingForComplete = true;
            return Temp;
        }

        public string PopupTrinary(string message, string title, string[] Buttons)
        {
            UIAlertController okCancelAlertController = null;
            UIViewController vc = null;
            string Temp = "";
            UIApplication.SharedApplication.InvokeOnMainThread(() =>
            {
                UIWindow window = UIApplication.SharedApplication.KeyWindow;
                vc = window.RootViewController;
                while (vc.PresentedViewController != null)
                {
                    vc = vc.PresentedViewController;
                }
                okCancelAlertController = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
                okCancelAlertController.AddAction(UIAlertAction.Create(Buttons[0], UIAlertActionStyle.Default, alert =>
                {
                    Temp = Buttons[0];
                    WaitingForComplete = false;
                })); okCancelAlertController.AddAction(UIAlertAction.Create(Buttons[1], UIAlertActionStyle.Default, alert =>
                {
                    Temp = Buttons[1];
                    WaitingForComplete = false;
                }));
                okCancelAlertController.AddAction(UIAlertAction.Create(Buttons[2], UIAlertActionStyle.Cancel, alert =>
                {
                    Temp = Buttons[2];
                    WaitingForComplete = false;
                }));
                vc.InvokeOnMainThread(() =>
                {
                    try
                    {
                        vc.PresentViewController(okCancelAlertController, true, CompletionFunction);
                    }
                    catch (Exception)
                    {
                    }
                });
            });
            while (WaitingForComplete)
            {
                Thread.Sleep(200);
            }
            WaitingForComplete = true;
            return Temp;
        }

        public void RequestBluetoothPermission()
        {
        }

        public void RequestLocationPermission()
        {
        }

        public byte[] ResizeImage(byte[] imageData, float width, float height)
        {
            UIImage originalImage = ImageFromByteArray(imageData);
            var Height = originalImage.Size.Height;
            var Width = originalImage.Size.Width;
            nfloat Temp;
            nfloat temp;
            if (Height > Width)
            {
                Temp = height;
                nfloat teiler = Height / height;
                temp = Width / teiler;
            }
            else
            {
                temp = width;
                nfloat teiler = Width / width;
                Temp = Height / teiler;
            }
            width = (float)temp;
            height = (float)Temp;
            UIGraphics.BeginImageContext(new SizeF(width, height));
            originalImage.Draw(new RectangleF(0, 0, width, height));
            var resizedImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            var ImageBytes = resizedImage.AsPNG().ToArray();
            resizedImage.Dispose();
            return ImageBytes;
        }

        public void SendTextEmail(string Topic, string data, string recipient)
        {
            UIWindow window = UIApplication.SharedApplication.KeyWindow;
            UIViewController vc = window.RootViewController;
            if (MFMailComposeViewController.CanSendMail)
            {
                mailController = new MFMailComposeViewController();
                mailController.SetToRecipients(new string[] { recipient });
                mailController.SetSubject(Topic);
                mailController.SetMessageBody(data, false);
                mailController.Finished += (object s, MFComposeResultEventArgs args) =>
                {
                    Console.WriteLine(args.Result.ToString());
                    args.Controller.DismissViewController(true, null);
                };
                vc.PresentViewController(mailController, true, null);
            }
        }

        public void ShareFile(string path)
        {
            UIWindow window = UIApplication.SharedApplication.KeyWindow;
            UIViewController vc = window.RootViewController;
            if (!File.Exists(path))
            {
                //Item does not exist? Show error message
                var alert = UIAlertController.Create("inSpec", "Unknown Error", UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));
                vc.PresentViewController(alert, true, null);
                return;
            }
            if (shareController != null)
            {
                shareController.DismissMenu(true);
                shareController = null;
            }
            else
            {
                shareController = new UIDocumentInteractionController()
                {
                    Url = NSUrl.FromFilename(path)
                };
                shareController.DidDismissOptionsMenu += (s, a) => shareController = null;
                var rect = (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone) ? vc.View.Bounds : new CGRect(0, vc.View.Bounds.Height - 200, vc.View.Bounds.Width, vc.View.Bounds.Height);
                if (shareController.PresentOptionsMenu(rect, vc.View, true))
                {
                    Console.WriteLine("MENU SHOWN");
                }
                else
                {
                    Console.WriteLine(("MENU NOT SHOWN!"));
                }
                shareController.WillBeginSendingToApplication += (sender1, e) =>
                {
                    Console.WriteLine("Sending to app");
                };
                shareController.WillPresentOpenInMenu += (sender3, e) => Console.WriteLine("Will present open in menu");
                shareController.DidEndPreview += (sender4, e) => Console.WriteLine("Did End Preview");
                shareController.DidDismissOpenInMenu += (sender5, e) =>
                {
                    Console.WriteLine("Ended open in menu");
                    shareController.Dispose();
                    shareController = null;
                };
                shareController.DidEndSendingToApplication += (sender2, e) =>
                {
                    Console.WriteLine("Done sending");
                };
            }
        }

        public void ShowToast(SKRect rectangleToDraw, string message)
        {
            try
            {
                UIWindow window = UIApplication.SharedApplication.KeyWindow;
                UIViewController vc = window.RootViewController;
                CGRect temp = new CGRect(rectangleToDraw.Left, rectangleToDraw.Top, rectangleToDraw.Width, rectangleToDraw.Height);
                while (vc.PresentedViewController != null)
                {
                    vc = vc.PresentedViewController;
                }
                var toastLabel = new UILabel(temp)
                {
                    BackgroundColor = UIColor.Black.ColorWithAlpha(0.6F),
                    TextColor = UIColor.White,
                    TextAlignment = UITextAlignment.Center,
                    Font = UIFont.FromName("Helvetica-Bold", 12.0F),
                    Text = message,
                    Alpha = 1.0F,
                    ClipsToBounds = true,
                    Lines = 3
                };
                toastLabel.Layer.CornerRadius = 10;
                vc.View.AddSubview(toastLabel);
                UIView.Animate(8.0, 0.1, UIViewAnimationOptions.CurveEaseOut, () => { toastLabel.Alpha = 0; }, () => { });
            }
            catch
            {
            }
        }

        public byte[] SKImageToByte(SKImage Image)
        {
            return Image.ToUIImage().AsPNG().ToArray();
        }

        private void CompletionFunction()
        {
        }

        private void GetLocationConsent()
        {
            using (CLLocationManager manager = new CLLocationManager())
            {
                manager.AuthorizationChanged += (sender, args) =>
                {
                    Console.WriteLine("Authorization changed to: {0}", args.Status);
                };
                if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
                    manager.RequestWhenInUseAuthorization();
            }
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