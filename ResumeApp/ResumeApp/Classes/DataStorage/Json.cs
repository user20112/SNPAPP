using Newtonsoft.Json;
using ResumeApp.Classes;
using ResumeApp.DataClasses;
using ResumeApp.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace ResumeApp.DataStorage
{
    public class Json : IDataStorage
    {
        private readonly bool UsesImages = true;

        public Json(IFileSave filesave, ISettingsManager settingsManager, bool usesImages)
        {
            UsesImages = usesImages;
            SettingsManager = settingsManager;
            FileSave = filesave;
            if (!Directory.Exists(System.IO.Path.Combine(filesave.DefaultSaveLocation, "Default Project")))
                Directory.CreateDirectory(System.IO.Path.Combine(filesave.DefaultSaveLocation, "Default Project"));
            Path = System.IO.Path.Combine(System.IO.Path.Combine(filesave.DefaultSaveLocation, "Default Project"), "ProjectData.json");
            Refresh();
        }

        public Dictionary<string, ColorCode> DictionaryOfColorCodes { get; set; }

        public Dictionary<string, string> DictionaryOfOptions { get; set; }

        public List<FiberType> FiberTypes { get; set; }

        public IFileSave FileSave { get; set; }

        public List<IECSpec> IECSpecs { get; set; }

        public List<InspectionSaveData> ListOfInspections { get; set; }

        public List<Reading> ListOfReadings { get; set; }

        public Dictionary<string, int> MiscStorage { get; set; }

        public string Path { get; set; }

        public ISettingsManager SettingsManager { get; set; }

        public void Clear()
        {
            ListOfInspections.Clear();
            ListOfReadings.Clear();
            MiscStorage.Clear();
        }

        public void ClearOptions()
        {
            DictionaryOfOptions = new Dictionary<string, string>();
        }

        public bool ContainsData()
        {
            if (DoesExist())
            {
                var theDataStorage = File.ReadAllText(Path);
                if (!string.IsNullOrEmpty(theDataStorage))
                {
                    return true;
                }
            }
            return false;
        }

        public void Create()
        {
            if (!File.Exists(Path)) File.Create(Path).Close();
        }

        public void Delete(InspectionSaveData Inspec)
        {
            for (int i = 0; i < ListOfInspections.Count; i++)
            {
                if (ListOfInspections[i].FileSaveName == Inspec.FileSaveName)
                {
                    ListOfInspections.Remove(ListOfInspections[i]);
                    i--;
                }
            }
        }

        public bool DoesExist()
        {
            if (File.Exists(Path))
            {
                return true;
            }
            return false;
        }

        public bool GenerateOffloadZip(string OutputPath)
        {
            try
            {
                string PathToZip = System.IO.Path.Combine(FileSave.DefaultSaveLocation, "Default Project");
                ZipFile.CreateFromDirectory(PathToZip, OutputPath, CompressionLevel.Optimal, true);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GetData()
        {
            if (File.Exists(Path))
            {
                string data = File.ReadAllText(Path);
                return data;
            }
            return "";
        }

        public void OverWrite(InspectionSaveData Inspec)
        {
            for (int i = 0; i < ListOfInspections.Count; i++)
            {
                if (ListOfInspections[i].FileSaveName == Inspec.FileSaveName)
                {
                    ListOfInspections[i] = Inspec;
                }
            }
        }

        public void OverWrite(Reading Reading)
        {
            for (int i = 0; i < ListOfReadings.Count; i++)
            {
                if (ListOfReadings[i].ID == Reading.ID)
                {
                    ListOfReadings[i] = Reading;
                }
            }
        }

        public void Refresh()
        {
            FiberTypes = new List<FiberType>
            {
                new FiberType("Default", 40, 40),
                new FiberType("MPO", 75, 40),
                new FiberType("2.5 MM", 40, 40),
                new FiberType("1.25 MM", 40, 40),
                new FiberType("E2000", 40, 40),
                new FiberType("OptiTap", -50, -25)
            };
            Console.WriteLine("IECSpecs Not Null");
            IECSpecs = new List<IECSpec>
            {
               IECSpec.Defualt(),
                 new IECSpec(100, 5, 10, 0, 45, 55, 40, 50, 27, 30, 2, "APC SM RL≥45 DB"),
                 new IECSpec(100, 5, 10, 2, 45, 55, 40, 50, 27, 30, 2, "PC SM RL≥26 DB"),
                 new IECSpec(100, 4, 10, 0, 45, 55, 40, 50, 27, 30, 2, "PC MultiMode")
            };
            DictionaryOfColorCodes = FileSave.GetColorCodes();
            try
            {
                DictionaryOfColorCodes.Add("None", new ColorCode(new List<string>(), new List<string>(), new List<string>(), new List<string>(), "None"));
            }
            catch
            {
            }
            ListOfReadings = new List<Reading>();
            ListOfInspections = new List<InspectionSaveData>();
            DictionaryOfOptions = new Dictionary<string, string>();
            MiscStorage = new Dictionary<string, int>();
            try
            {
                string JsonData = GetData();
                if (!string.IsNullOrWhiteSpace(JsonData))
                {
                    string[] JsonArray = JsonData.Split(new string[] { "|||" }, StringSplitOptions.None);
                    ListOfReadings = JsonConvert.DeserializeObject<List<Reading>>(JsonArray[1]);
                    DictionaryOfOptions = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonArray[2]);
                    MiscStorage = JsonConvert.DeserializeObject<Dictionary<string, int>>(JsonArray[3]);
                    ListOfInspections = JsonConvert.DeserializeObject<List<InspectionSaveData>>(JsonArray[0]);
                    if (UsesImages)
                        foreach (InspectionSaveData inspection in ListOfInspections)
                            try
                            {
                                inspection.Image = FileSave.SKImageFromFile(inspection.FileSaveName);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                            }
                    if (ListOfInspections == null)
                        ListOfInspections = new List<InspectionSaveData>();
                    if (ListOfReadings == null)
                        ListOfReadings = new List<Reading>();
                    if (DictionaryOfOptions == null)
                        DictionaryOfOptions = new Dictionary<string, string>();
                    if (MiscStorage == null)
                        MiscStorage = new Dictionary<string, int>();
                }
            }
            catch
            {
            }
        }

        public void SaveOptions(Dictionary<string, string> Options)
        {
            try
            {
                ClearOptions();
                foreach (KeyValuePair<string, string> entry in Options)
                {
                    try
                    {
                        DictionaryOfOptions.Add(entry.Key, entry.Value);
                    }
                    catch
                    {
                    }
                }
                Update();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void SetDefaultOptions()
        {
            DictionaryOfOptions.Add("Color Code", "None");
            DictionaryOfOptions.Add("Report Type", "PDF");
            DictionaryOfOptions.Add("Fiber Type", "Default");
            DictionaryOfOptions.Add("MFBFunction", "Focus And Analyze");
            DictionaryOfOptions.Add("AutoSaveFunction", "Never");
            DictionaryOfOptions.Add("IECSpec", "UPC SM RL>=45 DB");
        }

        public void Store(string key, int value)
        {
            try
            {
                MiscStorage.Add(key, value);
            }
            catch
            {
                try
                {
                    MiscStorage.Remove(key);
                    MiscStorage.Add(key, value);
                }
                catch
                {
                }
            }
            Update();
        }

        public void Store(Reading Reading)
        {
            ListOfReadings.Add(Reading);
            Update();
        }

        public void Store(string OptionKey, string OptionValue)
        {
            DictionaryOfOptions.Add(OptionKey, OptionValue);
            Update();
        }

        public void Store(InspectionSaveData Inspec)
        {
            ListOfInspections.Add(Inspec);
            Update();
        }

        public void Update(List<Reading> source)
        {
            try
            {
                Refresh();
                ListOfReadings = source;
                Update();
            }
            catch (Exception)
            {
                Console.WriteLine("Error occured creating new csv file");
            }
        }

        public void Update(List<InspectionSaveData> source)
        {
            try
            {
                Refresh();
                ListOfInspections = source;
                Update();
            }
            catch (Exception)
            {
                Console.WriteLine("Error occured creating new csv file");
            }
        }

        public void Update(List<InspectionSaveData> source, List<Reading> source2)
        {
            try
            {
                Refresh();
                ListOfReadings = source2;
                ListOfInspections = source;
                Update();
            }
            catch (Exception)
            {
                Console.WriteLine("Error occured creating new csv file");
            }
        }

        public void Update()
        {
            try
            {
                if (DoesExist())
                    File.Delete(Path);
                using StreamWriter file = File.CreateText(Path);
                file.Write(JsonConvert.SerializeObject(ListOfInspections));
                file.Write("|||");
                file.Write(JsonConvert.SerializeObject(ListOfReadings));
                file.Write("|||");
                file.Write(JsonConvert.SerializeObject(DictionaryOfOptions));
                file.Write("|||");
                file.Write(JsonConvert.SerializeObject(MiscStorage));
                file.Write("|||");
            }
            catch
            {
            }
        }

        public void UpdateOption(string key, string value)
        {
            try
            {
                try
                {
                    DictionaryOfOptions.Remove(key);
                }
                catch
                {
                }
                DictionaryOfOptions.Add(key, value);
                Update();
            }
            catch
            {
            }
        }

        public void UpdateOptions(List<string> keys, List<string> values)
        {
            for (int x = 0; x < keys.Count; x++)
            {
                try
                {
                    try
                    {
                        DictionaryOfOptions.Remove(keys[x]);
                    }
                    catch
                    {
                    }
                    DictionaryOfOptions.Add(keys[x], values[x]);
                }
                catch
                {
                }
            }
            Update();
        }
    }
}