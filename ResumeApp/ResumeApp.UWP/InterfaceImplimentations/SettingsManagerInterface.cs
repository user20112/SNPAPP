using Newtonsoft.Json;
using ResumeApp.Interfaces;
using ResumeApp.UWP.InterfaceImplimentations;
using System.Collections.Generic;
using System.IO;

[assembly: Xamarin.Forms.Dependency(typeof(SettingsManagerInterface))]

namespace ResumeApp.UWP.InterfaceImplimentations
{
    internal class SettingsManagerInterface : ISettingsManager
    {
        private readonly FileSaveInterface FileSave;
        private Dictionary<string, string> Settings = new Dictionary<string, string>();

        public SettingsManagerInterface()
        {
            FileSave = new FileSaveInterface();
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
            RefreshSettings();
            if (!Settings.TryGetValue(toGet, out string Return))
            {
                Return = defaultValue;
            }
            return Return;
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
    }
}