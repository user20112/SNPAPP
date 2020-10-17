using ResumeApp.Events;
using System;
using System.Threading;

namespace ResumeApp.Interfaces
{
    public interface OPM : IDisposable
    {
        public string CurrentMeasurement { get; set; }
        public string CurrentMode { get; set; }
        public double CurrentReference { get; set; }
        public string CurrentTone { get; set; }
        public string CurrentWavelength { get; set; }
        public bool Live { get; set; }
        public string Name { get; set; }
        public EventHandler<OffLoadEventArgs> OnOffLoad { get; set; }
        public EventHandler<ReadingChangedEventArgs> OnReadingChanged { get; set; }

        public abstract void ChangeWaveLength(int waveLength);

        public abstract void DBDBM();

        public abstract void SetReference(string ButtonText);

        public abstract void StartStream(EventHandler<ReadingChangedEventArgs> OnReadingChanged, SynchronizationContext Context, EventHandler<OffLoadEventArgs> OnOffload);

        public abstract void StopStream();
    }
}