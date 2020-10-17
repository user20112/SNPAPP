using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Hardware.Usb;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using ResumeApp.Droid.CallBacks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ResumeApp.Droid
{
    [Activity(Label = "ResumeApp", Icon = "@drawable/icon", Theme = "@style/MyTheme.Splash", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private List<int> ActiveRequests = new List<int>();
        private int RequestCode = 1;
        private List<Tuple<bool, int>> RequestReturns = new List<Tuple<bool, int>>();
        private USBPermissionBroadcastReceiver USBPermissionBroadcastReceiver;
        internal static MainActivity Instance { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.SetTheme(Resource.Style.MainTheme);
            App.ScreenHeight = (int)(Resources.DisplayMetrics.HeightPixels / Resources.DisplayMetrics.Density);
            App.ScreenWidth = (int)(Resources.DisplayMetrics.WidthPixels / Resources.DisplayMetrics.Density);
            Instance = this;
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            RequestedOrientation = ScreenOrientation.Portrait;
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }

        public bool CheckPermission(string permission, SynchronizationContext Main)
        {
            Permission PermCode = CheckSelfPermission(permission);
            return PermCode == Permission.Granted;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            for (int x = 0; x < ActiveRequests.Count; x++)
            {
                if (ActiveRequests[x] == requestCode)
                {
                    RequestReturns.Add(new Tuple<bool, int>(grantResults[0] == Permission.Granted, requestCode));
                    ActiveRequests.RemoveAt(x);
                }
            }
        }

        public bool RequestPermissions(string[] permissions, SynchronizationContext Main)
        {
            int CurrentRequestCode = RequestCode++;
            List<string> Permissions = new List<string>();
            foreach (string x in permissions)
            {
                if (!CheckPermission(x, Main))
                    Permissions.Add(x);
            }
            permissions = Permissions.ToArray();
            if (permissions.Count() > 0)
            {
                ActiveRequests.Add(CurrentRequestCode);
                Main.Send(delegate { ActivityCompat.RequestPermissions(this, permissions, CurrentRequestCode); }, null);
                Tuple<bool, int> ReturnValue = null;
                bool Waiting = true;
                while (Waiting)
                {
                    Thread.Sleep(50);
                    for (int x = 0; x < RequestReturns.Count; x++)
                    {
                        if (RequestReturns[x].Item2 == CurrentRequestCode)
                        {
                            Waiting = false;
                            ReturnValue = RequestReturns[x];
                        }
                    }
                }
                RequestReturns.Remove(ReturnValue);
                return ReturnValue.Item1;
            }
            else
            {//all already granted.
                return true;
            }
        }

        public bool RequestUSBPermissions(UsbManager Manager, UsbDevice device, SynchronizationContext Main)
        {
            bool Failed = false;
            int CurrentRequestCode = RequestCode++;
            bool Granted = false;
            Main.Send(delegate
            {
                try
                {
                    if (!Manager.HasPermission(device))
                    {
                        ActiveRequests.Add(CurrentRequestCode);
                        USBPermissionBroadcastReceiver.SetupRequest(CurrentRequestCode, "com.odm_inc.inSpec.usb.host");
                        PendingIntent mPermissionIntent = PendingIntent.GetBroadcast(this, CurrentRequestCode, new Intent("com.odm_inc.inSpec.usb.host"), 0);
                        Manager.RequestPermission(device, mPermissionIntent);
                    }
                    else
                    {
                        Granted = true;
                    }
                }
                catch (Exception ex)
                {
                    Failed = true;
                }
            }, null);
            if (Granted)
                return true;
            if (Failed)
                return false;
            Tuple<bool, int> ReturnValue = null;
            bool Waiting = true;
            while (Waiting)
            {
                Thread.Sleep(50);
                for (int x = 0; x < RequestReturns.Count; x++)
                {
                    if (RequestReturns[x].Item2 == CurrentRequestCode)
                    {
                        Waiting = false;
                        ReturnValue = RequestReturns[x];
                    }
                }
            }
            RequestReturns.Remove(ReturnValue);
            return ReturnValue.Item1;
        }
    }
}