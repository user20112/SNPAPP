using Android.Content;
using ResumeApp.Droid.PlatformSpecific;
using ResumeApp.Interfaces;

[assembly: Xamarin.Forms.Dependency(typeof(SettingsManagerInterface))]

namespace ResumeApp.Droid.PlatformSpecific
{
    public class SettingsManagerInterface : ISettingsManager
    {
        public ISharedPreferences settings;
        public ISharedPreferencesEditor Settings;

        public SettingsManagerInterface()
        {
            settings = Android.App.Application.Context.GetSharedPreferences("Settings", 0);
            Settings = settings.Edit();
        }

        public bool GetBool(string ToGet, bool Default)
        {
            return settings.GetBoolean(ToGet, Default);
        }

        public string GetString(string toGet, string defaultValue = "")
        {
            return settings.GetString(toGet, defaultValue);
        }

        public void SetBool(bool isChecked, string BoolToSet)
        {
            Settings.PutBoolean(BoolToSet, isChecked);
            Settings.Apply();
            settings = Android.App.Application.Context.GetSharedPreferences("Settings", 0);
        }

        public void SetString(string Value, string StringToSet)
        {
            Settings.PutString(StringToSet, Value);
            Settings.Apply();
            settings = Android.App.Application.Context.GetSharedPreferences("Settings", 0);
        }
    }
}