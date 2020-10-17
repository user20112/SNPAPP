using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using System;

namespace ResumeApp.Droid.CallBacks
{
    [BroadcastReceiver(Enabled = true, Exported = false)]
    [IntentFilter(new[] { "com.odm_inc.inSpec.usb.host" })]
    internal class USBPermissionBroadcastReceiver : BroadcastReceiver
    {
        private int CurrentRequestCode = 0;
        private Action<int, string[], Permission[]> onRequestPermissionsResult;

        private string RequestString = "";

        public USBPermissionBroadcastReceiver()
        {
        }

        public USBPermissionBroadcastReceiver(Action<int, string[], Permission[]> onRequestPermissionsResult)
        {
            this.onRequestPermissionsResult = onRequestPermissionsResult;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            Bundle bundle = intent.Extras;
            if (bundle != null)
            {
                foreach (string key in bundle.KeySet())
                {
                    Console.WriteLine(key + " : " + (bundle.Get(key) != null ? bundle.Get(key) : "NULL"));
                }
            }
            onRequestPermissionsResult(CurrentRequestCode, new string[1] { RequestString }, new Permission[1] { ((Boolean)bundle.Get("permission")) ? Permission.Granted : Permission.Denied });
        }

        public void SetupRequest(int currentRequestCode, string v)
        {
            RequestString = v;
            CurrentRequestCode = currentRequestCode;
        }
    }
}