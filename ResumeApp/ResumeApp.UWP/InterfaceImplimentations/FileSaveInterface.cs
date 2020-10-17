using ResumeApp.Classes;
using ResumeApp.Interfaces;
using ResumeApp.Resources;
using ResumeApp.UWP.InterfaceImplimentations;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Windows.Storage;

[assembly: Xamarin.Forms.Dependency(typeof(FileSaveInterface))]

namespace ResumeApp.UWP.InterfaceImplimentations
{
    internal class FileSaveInterface : IFileSave
    {
        private StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        public string DefaultSaveLocation { get { return localFolder.Path; } set => throw new NotImplementedException(); }

        public FileSaveInterface()
        {
            string Temp = Path.Combine(DefaultSaveLocation, "Settings");
            if (!Directory.Exists(Temp))
                Directory.CreateDirectory(Temp);
            Temp = Path.Combine(DefaultSaveLocation, "ColorCodes");
            if (!Directory.Exists(Temp))
                Directory.CreateDirectory(Temp);
            if (!File.Exists(Path.Combine(Temp, "CSpireEricsson")))
            {
                SaveColorCode("CSpireEricsson", ColorCodeResources.CSpireEricsson);
                SaveColorCode("SamsungClearwire", ColorCodeResources.SamsungClearwire);
                SaveColorCode("SprintAlcatelLucent", ColorCodeResources.SprintAlcatelLucent);
                SaveColorCode("SprintALU", ColorCodeResources.SprintALU);
                SaveColorCode("SprintEricsson", ColorCodeResources.SprintEricsson);
                SaveColorCode("SprintNokiaMIMO", ColorCodeResources.SprintNokiaMIMO);
                SaveColorCode("SprintNSN", ColorCodeResources.SprintNSN);
                SaveColorCode("SprintSamsung", ColorCodeResources.SprintSamsung);
                SaveColorCode("SprintSamsungExistingPost", ColorCodeResources.SprintSamsungExistingPost);
                SaveColorCode("SprintSamsungExistingPre", ColorCodeResources.SprintSamsungExistingPre);
                SaveColorCode("SprintSamsungHiCap", ColorCodeResources.SprintSamsungHiCap);
                SaveColorCode("SprintSamsungNewPost", ColorCodeResources.SprintSamsungNewPost);
                SaveColorCode("SprintSamsungNewPre", ColorCodeResources.SprintSamsungNewPre);
                SaveColorCode("TMobile", ColorCodeResources.TMobile);
                SaveColorCode("TMobileEricsson", ColorCodeResources.TMobileEricsson);
                SaveColorCode("TMobileEricsson5G", ColorCodeResources.TMobileEricsson5G);
                SaveColorCode("TMobileNokia", ColorCodeResources.TMobileNokia);
            }
        }

        public void SaveColorCode(string Name, string Value)
        {
            string Documents = DefaultSaveLocation;
            string DirectoryName = Path.Combine(Documents, "ColorCodes");
            if (!Directory.Exists(DirectoryName))//if the project doesnt exist create a directory for it
                Directory.CreateDirectory(DirectoryName);
            FileStream files = File.Create(Path.Combine(DirectoryName, FileSafe(Name)));
            files.Write(Encoding.ASCII.GetBytes(Value), 0, Value.Length);
            files.Close();
        }

        public void Delete(string path)
        {
            string Documents = DefaultSaveLocation;
            string DirectoryName = Path.Combine(Documents, path);
            File.Delete(DirectoryName);
        }

        public string FileSafe(string path)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
            { path = path.Replace(c, '-'); }
            return path;
        }

        public string GetAppResource(string EndingPath)
        {
            string returndata = "";
            EndingPath = Path.Combine("ResumeAppResources", EndingPath);
            using (StreamReader sr = new StreamReader(Path.Combine(DefaultSaveLocation, EndingPath)))
            {
                returndata = sr.ReadToEnd();
            }
            return returndata;
        }

        public Dictionary<string, ColorCode> GetColorCodes()
        {
            Dictionary<string, ColorCode> returnData = new Dictionary<string, ColorCode>();
            string DirectoryName = Path.Combine(DefaultSaveLocation, "ColorCodes");
            if (!Directory.Exists(DirectoryName))//if the directory does not exist create it
            {
                Directory.CreateDirectory(DirectoryName);
            }
            foreach (string file in new List<string>(Directory.EnumerateFiles(DirectoryName)))
            {
                try
                {
                    List<string> GroupingColors = new List<string>();
                    List<string> IndividualColors = new List<string>();
                    List<string> Sectors = new List<string>();
                    List<string> Cores = new List<string>();
                    string contents = File.ReadAllText(file);
                    string[] Colors = contents.Split('-');
                    foreach (string Color in Colors)
                    {
                        if (!string.IsNullOrWhiteSpace(Color))
                        {
                            string[] Values = Color.Split('/');
                            Sectors.Add(Values[0]);
                            GroupingColors.Add(Values[1]);
                            IndividualColors.Add(Values[2]);
                            Cores.Add(Values[3]);
                        }
                    }
                    ColorCode CurrentCode = new ColorCode(GroupingColors, IndividualColors, Sectors, Cores, Path.GetFileNameWithoutExtension(file));
                    returnData.Add(CurrentCode.ColorCodeName, CurrentCode);
                }
                catch
                {
                    Console.WriteLine("Error Loading ColorCodes ");
                }
            }
            return returnData;
        }

        public string GetResource(string name)
        {
            string ResourceString = "";
            using (StreamReader sr = new StreamReader(Path.Combine(DefaultSaveLocation, name)))
            {
                ResourceString = sr.ReadToEnd();
            }
            return ResourceString;
        }

        public string OverFile(string PathName, byte[] file)
        {
            try
            {
                File.WriteAllBytes(PathName, file);
                return PathName;
            }
            catch
            {
                return "";
            }
        }

        public string OverFile(string PathName, Stream file)
        {
            try
            {
                FileStream fileStream = File.Create(PathName);
                file.CopyTo(fileStream);
                fileStream.Close();
                return PathName;
            }
            catch
            {
                return "";
            }
        }

        public string OverImage(SKImage ImageToSave, string path)
        {
            try
            {
                string Documents = DefaultSaveLocation;
                if (!Directory.Exists(Documents))//if the project doesnt exist create a directory for it
                    Directory.CreateDirectory(Documents);
                string FullPathName = Documents + "/" + path + ".JPEG";
                if (OverFile(FullPathName, ImageToSave.Encode(SKEncodedImageFormat.Png, 100).ToArray()) != "")//using jpeg then stream stops it from generating a new Byte array and is a bit cheaper.
                {
                    return path;
                }
                else
                    return "";
            }
            catch
            {
                return "";
            }
        }

        public string SaveFile(string PathName, byte[] file)
        {
            try
            {
                if (File.Exists(PathName))
                {
                    return "";
                }
                File.WriteAllBytes(PathName, file);
                return PathName;
            }
            catch
            {
                return "";
            }
        }

        public string SaveFile(string PathName, Stream file)
        {
            try
            {
                if (File.Exists(PathName))
                {
                    return "";
                }
                FileStream fileStream = File.Create(PathName);
                file.CopyTo(fileStream);
                fileStream.Close();
                return PathName;
            }
            catch
            {
                return "";
            }
        }

        public string SaveImage(SKImage ImageToSave, string path)
        {
            try
            {
                string Documents = DefaultSaveLocation;
                if (!Directory.Exists(Documents))//if the project doesnt exist create a directory for it
                    Directory.CreateDirectory(Documents);
                string FullPathName = Documents + "/" + path + ".JPEG";
                if (SaveFile(FullPathName, ImageToSave.Encode(SKEncodedImageFormat.Png, 100).ToArray()) != "")//using jpeg then stream stops it from generating a new Byte array and is a bit cheaper.
                {
                    return path;
                }
                else
                    return "";
            }
            catch
            {
                return "";
            }
        }

        public SKBitmap SKBitmapFromAppFile(string PathName)
        {
            PathName = Path.Combine("AppShellResources", PathName);
            return SKBitmap.FromImage(SKImage.FromEncodedData(Path.Combine(DefaultSaveLocation, PathName)));
        }

        public SKBitmap SKBitmapFromFile(string PathName)
        {
            return SKBitmap.FromImage(SKImage.FromEncodedData(Path.Combine(DefaultSaveLocation, PathName)));
        }

        public SKImage SKImageFromResource(string Name)
        {
            return SKImage.FromEncodedData(Path.Combine(DefaultSaveLocation, Name));
        }

        public SKImage SKImageFromFile(string Name)
        {
            return SKImage.FromBitmap(SKBitmapFromFile(Name));
        }
    }
}