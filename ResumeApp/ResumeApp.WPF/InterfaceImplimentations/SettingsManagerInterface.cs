using Newtonsoft.Json;
using ResumeApp.Interfaces;
using ResumeApp.WPF.InterfaceImplimentations;
using System.Collections.Generic;
using System.IO;

[assembly: Xamarin.Forms.Dependency(typeof(SettingsManagerInterface))]

namespace ResumeApp.WPF.InterfaceImplimentations
{
    public class SettingsManagerInterface : ISettingsManager
    {
        private readonly FileSaveInterface FileSave = new FileSaveInterface();
        private Dictionary<string, string> Settings = new Dictionary<string, string>();

        public SettingsManagerInterface()
        {
            RefreshSettings();
        }

        public bool GetBool(string ToGet, bool Default)
        {
            if (!Settings.TryGetValue(ToGet, out string Return))
            {
                Return = Default.ToString();
            }
            return bool.Parse(Return);
        }

        public string GetString(string toGet, string defaultValue = "")
        {
            if (!Settings.TryGetValue(toGet, out string Return))
            {
                Return = defaultValue;
            }
            return Return;
        }

        public bool IsGPSLocationRequested()
        {
            return GetBool("GPSRequested", false);
        }

        public void SetBool(bool isChecked, string BoolToSet)
        {
            try
            {
                Settings.Add(BoolToSet, isChecked.ToString());
            }
            catch
            {
                Settings.Remove(BoolToSet);
                Settings.Add(BoolToSet, isChecked.ToString());
            }
            UpdateSettings();
        }

        public void SetString(string Value, string StringToSet)
        {
            try
            {
                Settings.Add(StringToSet, Value);
            }
            catch
            {
                Settings.Remove(StringToSet);
                Settings.Add(StringToSet, Value);
            }
            UpdateSettings();
        }

        public void SetupMinMax()
        {
        }

        private void RefreshSettings()
        {
            string settings = Path.Combine(FileSave.DefaultSaveLocation, "Settings", "Settings.txt");
            if (File.Exists(settings))
            {
                Settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(settings));
            }
        }

        private void UpdateSettings()
        {
            string settings = Path.Combine(FileSave.DefaultSaveLocation, "Settings", "Settings.txt");
            if (File.Exists(settings))
                File.Delete(settings);
            File.WriteAllText(settings, JsonConvert.SerializeObject(Settings));
        }
    }
}