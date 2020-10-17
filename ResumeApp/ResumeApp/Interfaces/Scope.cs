using ResumeApp.Classes;
using ResumeApp.Events;
using SkiaSharp;
using System;
using System.Threading;

namespace ResumeApp.Interfaces
{
    public abstract class Scope : IDisposable
    {
        public bool AutoAnalyzeSupported = false;
        public bool Connected = false;
        public CoreCoord CoreLocation = new CoreCoord();
        public SKImage CurrentImage = null;
        public int FrameCounter = 0;
        public EventHandler<FrameReadyEventArgs> FrameReadyEvents;
        public bool KeepFocusing = true;
        public EventHandler<EventArgs> MFBPressedEvents;
        public string Name = "";
        public bool Paused = false;
        public bool SupportsFocus = false;
        public bool zoom = false;

        public abstract void AfterAnalysis(bool Passed, bool Completed);

        public abstract void Center(bool Toggle, EventHandler<DisplayEventArgs> Display, SynchronizationContext Main);

        public abstract void Dispose();

        public abstract void PauseStream(bool toggle);

        public abstract void ReCenter(EventHandler<DisplayEventArgs> Display, SynchronizationContext Main);

        public abstract void StartStream(EventHandler<FrameReadyEventArgs> OnFrame, SynchronizationContext Context, EventHandler<EventArgs> ButtonPressed);

        public abstract void StopStream();
    }
}