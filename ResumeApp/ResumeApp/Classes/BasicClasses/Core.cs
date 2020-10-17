using Newtonsoft.Json;
using SkiaSharp;
using System;

namespace ResumeApp.Classes
{
    public class Core : IComparable<Core>
    {
        public AnalysisData AnalysisData;
        public CoreCoord CoreCoord;

        [JsonIgnore]
        public SKImage CoreImage;

        public int Polish;

        public Core(SKImage mpoimage, CoreCoord coreCoord, int polish, AnalysisData analysisData)
        {
            AnalysisData = analysisData;
            CoreImage = mpoimage;
            CoreCoord = coreCoord;
            Polish = polish;
        }

        public int CompareTo(Core other)
        {
            return CoreCoord.center.X.CompareTo(other.CoreCoord.center.X);
        }
    }
}