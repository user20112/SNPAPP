using ResumeApp.Classes;
using ResumeApp.DataClasses;
using System.Collections.Generic;

namespace ResumeApp.Interfaces
{
    public interface IDataStorage
    {
        Dictionary<string, ColorCode> DictionaryOfColorCodes { get; set; }
        Dictionary<string, string> DictionaryOfOptions { get; set; }
        List<FiberType> FiberTypes { get; set; }
        IFileSave FileSave { get; set; }
        List<IECSpec> IECSpecs { get; set; }
        List<InspectionSaveData> ListOfInspections { get; set; }
        List<Reading> ListOfReadings { get; set; }
        Dictionary<string, int> MiscStorage { get; set; }
        string Path { get; set; }
        ISettingsManager SettingsManager { get; set; }

        void Clear();

        void ClearOptions();

        bool ContainsData();

        void Create();

        void Delete(InspectionSaveData Inspec);

        bool DoesExist();

        bool GenerateOffloadZip(string OutputPath);

        string GetData();

        void OverWrite(InspectionSaveData Insepc);

        void OverWrite(Reading Reading);

        void Refresh();

        void SaveOptions(Dictionary<string, string> Options);

        void SetDefaultOptions();

        void Store(Reading Reading);

        void Store(string OptionKey, string OptionValue);

        void Store(InspectionSaveData Inspec);

        void Store(string key, int value);

        void Update(List<Reading> source);

        void Update(List<InspectionSaveData> source);

        void Update(List<InspectionSaveData> source, List<Reading> source2);

        void Update();

        void UpdateOption(string key, string value);

        void UpdateOptions(List<string> keys, List<string> values);
    }
}