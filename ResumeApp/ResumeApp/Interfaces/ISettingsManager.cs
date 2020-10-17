namespace ResumeApp.Interfaces
{
    public interface ISettingsManager
    {
        bool GetBool(string ToGet, bool Default);

        string GetString(string toGet, string defaultValue = "");

        void SetBool(bool isChecked, string BoolToSet);

        void SetString(string Value, string StringToSet);
    }
}