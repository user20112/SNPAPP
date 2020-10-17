using Foundation;
using ResumeApp.Interfaces;
using ResumeApp.iOS.PlatformSpecific;
using System;

[assembly: Xamarin.Forms.Dependency(typeof(SettingsManagerInterface))]

namespace ResumeApp.iOS.PlatformSpecific
{
    public class SettingsManagerInterface : ISettingsManager
    {
        public NSUserDefaults userSettings = NSUserDefaults.StandardUserDefaults;

        public bool GetBool(string ToGet, bool Default)
        {
            try
            {
                return userSettings.BoolForKey(ToGet);
            }
            catch { return Default; }
        }

        public string GetString(string toGet, string defaultValue = "")
        {
            try
            {
                var gotString = userSettings.StringForKey(toGet);
                if (string.IsNullOrWhiteSpace(gotString)) throw new ArgumentException();
                return gotString;
            }
            catch { return defaultValue; };
        }

        public void SetBool(bool isChecked, string BoolToSet)
        {
            userSettings.SetBool(isChecked, BoolToSet);
            userSettings = NSUserDefaults.StandardUserDefaults;
        }

        public void SetString(string Value, string StringToSet)
        {
            userSettings.SetString(Value, StringToSet);
            userSettings = NSUserDefaults.StandardUserDefaults;
        }
    }
}