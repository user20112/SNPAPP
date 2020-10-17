using ResumeApp.Classes;
using ResumeApp.Events;
using System;
using System.Collections.Generic;

namespace ResumeApp.Interfaces
{
    public abstract class IVideoSource : IDisposable
    {
        public bool ButtonAvailable;
        public SkiaSharp.SKImage CurrentImage;
        public int FrameCounter;
        public EventHandler<FrameReadyEventArgs> FrameReady;
        public EventHandler<EventArgs> OnMFB;
        public bool Paused;
        public int TimeElapsedSinceLastFrame;

        public abstract void Load(string FriendlyName, int FrameRate, int Height, int Width, string Encoding);

        public abstract List<VideoType> ScanDevice(string friendlyName);

        public abstract List<string> Scan();

        public abstract void Dispose();

        public abstract void PauseStream(bool Toggle);

        public abstract void StartStream();

        public abstract void StopStream();
    }
}