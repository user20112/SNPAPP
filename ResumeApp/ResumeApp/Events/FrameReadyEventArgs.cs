using SkiaSharp;
using System;

namespace ResumeApp.Events
{
    public class FrameReadyEventArgs : EventArgs
    {
        public byte[] FrameBuffer;//  byte[] of the image.
        public SKImage Image;//  non platform specific image
    }
}