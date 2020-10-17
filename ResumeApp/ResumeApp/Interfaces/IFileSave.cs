using ResumeApp.Classes;
using SkiaSharp;
using System.Collections.Generic;
using System.IO;

namespace ResumeApp.Interfaces
{
    public interface IFileSave
    {
        string DefaultSaveLocation { get; set; }

        void Delete(string path);

        Dictionary<string, ColorCode> GetColorCodes();

        string FileSafe(string path);

        string GetAppResource(string EndingPath);

        string GetResource(string name);

        string OverFile(string PathName, byte[] file);

        string OverFile(string PathName, Stream file);

        string OverImage(SKImage ImageToSave, string path);

        string SaveFile(string PathName, byte[] file);

        string SaveFile(string PathName, Stream file);

        string SaveImage(SKImage ImageToSave, string path);

        SKBitmap SKBitmapFromAppFile(string PathName);

        SKImage SKImageFromResource(string Name);

        SKImage SKImageFromFile(string Name);
    }
}