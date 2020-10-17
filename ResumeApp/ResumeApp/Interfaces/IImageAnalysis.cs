using ResumeApp.Classes;
using System.Collections.Generic;

namespace ResumeApp.Interfaces
{
    public interface IImageAnalysis
    {
        List<Core> AnalyzeImage(SkiaSharp.SKImage frameCaptureImage, int polish);

        CoreCoord FindClad(SkiaSharp.SKImage srcImage);

        CoreCoord FindCladFS(SkiaSharp.SKImage srcImage);
    }
}