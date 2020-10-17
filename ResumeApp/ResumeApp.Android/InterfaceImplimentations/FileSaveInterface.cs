using Android.Graphics;
using Emgu.CV;
using Emgu.CV.Structure;
using NativeFeatures.Droid;
using ResumeApp.Classes;
using ResumeApp.Droid;
using ResumeApp.Interfaces;
using ResumeApp.Resources;
using SkiaSharp;
using SkiaSharp.Views.Android;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Path = System.IO.Path;

[assembly: Xamarin.Forms.Dependency(typeof(FileSaveInterface))]

namespace NativeFeatures.Droid
{
    public class FileSaveInterface : IFileSave
    {
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

        public string DefaultSaveLocation
        {
            get => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            set => throw new NotImplementedException();
        }

        public static byte[] BitmaptoJPEG(Bitmap img)
        {
            using MemoryStream stream = new MemoryStream();
            img.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
            return stream.ToArray();
        }

        public void Delete(string path)
        {
            string Documents = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
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
            EndingPath = Path.Combine("AppResources", EndingPath);
            using (StreamReader sr = new StreamReader(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), EndingPath)))
            {
                returndata = sr.ReadToEnd();
            }
            return returndata;
        }

        public string GetResource(string name)
        {
            string ResourceString = "";
            using (StreamReader sr = new StreamReader(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), name)))
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
                string DirectoryName = Path.Combine(Documents, path);
                if (!Directory.Exists(DirectoryName))//if the project doesnt exist create a directory for it
                    Directory.CreateDirectory(DirectoryName);
                using Image<Bgr, byte> temp = Convert.SKImageToImage(ImageToSave);
                if (OverFile(DirectoryName, BitmaptoJPEG(temp.ToBitmap())) != "")//using jpeg then stream stops it from generating a new Byte array and is a bit cheaper.
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
                string DirectoryName = Path.Combine(Documents, path);
                if (!Directory.Exists(DirectoryName))//if the project doesnt exist create a directory for it
                    Directory.CreateDirectory(DirectoryName);
                using Image<Bgr, byte> temp = Convert.SKImageToImage(ImageToSave);
                if (SaveFile(DirectoryName, BitmaptoJPEG(temp.ToBitmap())) != "")//using jpeg then stream stops it from generating a new Byte array and is a bit cheaper.
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
            return BitmapFromResource(PathName).ToSKBitmap();
        }

        public SKBitmap SKBitmapFromFile(string PathName)
        {
            return SKBitmap.FromImage(BitmapFromFile(PathName).ToSKImage());
        }

        public SKImage SKImageFromResource(string Name)
        {
            return BitmapFromResource(Name).ToSKImage();
        }

        public Bitmap BitmapFromFile(string PathName)
        {
            string Documents = DefaultSaveLocation;
            string DirectoryName = Path.Combine(Documents, PathName);
            return BitmapFactory.DecodeFile(DirectoryName);
        }

        public Bitmap BitmapFromResource(string PathName)
        {
            try
            {
                string temp = Path.GetFileNameWithoutExtension(PathName.ToLower());
                int id = MainActivity.Instance.Resources.GetIdentifier(temp, "drawable", MainActivity.Instance.PackageName);
                MainActivity.Instance.GetDrawable(id);
                return BitmapFactory.DecodeResource(MainActivity.Instance.Resources, id);
            }
            catch
            {
                return null;
            }
        }

        public Dictionary<string, ColorCode> GetColorCodes()
        {
            Dictionary<string, ColorCode> returnData = new Dictionary<string, ColorCode>();
            string DirectoryName = Path.Combine(DefaultSaveLocation, "ColorCodes");
            if (!Directory.Exists(DirectoryName))//if the directory does not exist create it
            {
                Directory.CreateDirectory(DirectoryName);
                foreach (string file in new List<string>(Directory.EnumerateFiles(DirectoryName)))
                {
                    if (file != "ProjectData.json")
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
                            ColorCode CurrentCode = new ColorCode(GroupingColors, IndividualColors, Sectors, Cores, file.Substring(file.LastIndexOf("/") + 1, file.Length - file.LastIndexOf("/") - 1));
                            SaveColorCode(CurrentCode.ColorCodeName, CurrentCode);
                        }
                        catch
                        {
                            Console.WriteLine("Error Loading ColorCode " + file);
                        }
                    }
                }
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

        public string SaveColorCode(string name, ColorCode ColorCode)
        {
            try
            {
                string Documents = DefaultSaveLocation;
                string DirectoryName = Path.Combine(Documents, "ColorCodes");
                if (!Directory.Exists(DirectoryName))//if the project doesnt exist create a directory for it
                    Directory.CreateDirectory(DirectoryName);
                FileStream files = File.Create(Path.Combine(DirectoryName, FileSafe(name)));
                string data = "";
                string returndata = "Created File";
                for (int x = 0; x < ColorCode.Count; x++)
                {
                    data += ColorCode.Sectors[x] + "/";
                    data += ColorCode.GroupingColors[x] + "/";
                    data += ColorCode.IndividualColors[x] + "/";
                    data += ColorCode.Cores[x] + "";
                    data += "-";
                }
                returndata = "Generated Text" + data;
                files.Write(Encoding.ASCII.GetBytes(data), 0, data.Length);
                returndata = "Wrote Text" + data;
                files.Close();
            }
            catch (Exception ex)
            {
                return "Caught Exception:" + ex.ToString();
            }
            return "Saved ColorCode";
        }

        public SKImage SKImageFromFile(string Name)
        {
            return BitmapFromFile(Name).ToSKImage();
        }
    }
}