using ResumeApp.Events;
using ResumeApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xamarin.Essentials;

namespace ResumeApp.Classes.Devices
{
    internal class SNPOPM : OPM
    {
        private ICommunicationInterface ComInterface;

        public SNPOPM(ICommunicationInterface comInterface)
        {
            ComInterface = comInterface;
        }

        public string CurrentMeasurement { get; set; }
        public string CurrentMode { get; set; }
        public double CurrentReference { get; set; }
        public string CurrentTone { get; set; }
        public string CurrentWavelength { get; set; }
        public bool Live { get; set; }

        public string Name { get; set; } = "SNP OPM";
        public EventHandler<OffLoadEventArgs> OnOffLoad { get; set; }

        public EventHandler<ReadingChangedEventArgs> OnReadingChanged { get; set; }

        public void ChangeWaveLength(int waveLength)
        {
            ComInterface.WriteString("SetWav:" + waveLength.ToString() + ":");
        }

        public void DBDBM()
        {
            ComInterface.WriteString("TMode:");
        }

        public void Dispose()
        {
            try
            {
                StopStream();
                ComInterface.Dispose();
            }
            catch
            {
            }
        }

        public void SetReference(string ButtonText)
        {
            ComInterface.WriteString("Ref:");
        }

        public void StartStream(EventHandler<ReadingChangedEventArgs> OnReadingChanged, SynchronizationContext Context, EventHandler<OffLoadEventArgs> OnOffload)
        {
            if (!Live)
                ComInterface.OnReceive += OnMessageReceived;
            this.OnOffLoad += OnOffLoad;
            this.OnReadingChanged += OnReadingChanged;
            Live = true;
        }

        private bool Offloading = false;
        private List<Reading> ReadingsToOffload = new List<Reading>();

        private void OnMessageReceived(object sender, string e)
        {
            if (e.Contains("OffloadStart"))
            {
                Offloading = true;
                return;
            }
            if (e.Contains("OffloadStop"))
            {
                Offloading = false;
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    OnOffLoad.Invoke(this, new OffLoadEventArgs() { readings = ReadingsToOffload });
                    ReadingsToOffload.Clear();
                });
                return;
            }
            string[] split = e.Split('|');
            if (split.Count() == 6)
            {
                if (Offloading)
                {
                    ReadingsToOffload.Add(new Reading(split[0], split[2], split[1], "", Convert.ToSingle(split[3]), DateTime.Now, split[4]));
                }
                else
                {
                    HandleReading(new Reading(split[0], split[2], split[1], "", Convert.ToSingle(split[3]), DateTime.Now, split[5]));
                }
            }
            //measurement wavelength mode reference readingtosmall tone
        }

        public void HandleReading(Reading reading)
        {
            CurrentMeasurement = reading.Measurement;
            CurrentMode = reading.Unit;
            CurrentReference = reading.Reference;
            CurrentTone = reading.ToneDetected;
            CurrentWavelength = reading.Wavelength;
            MainThread.BeginInvokeOnMainThread(() =>
            {
                OnReadingChanged.Invoke(this, new ReadingChangedEventArgs() { reading = reading });
            });
        }

        public void StopStream()
        {
            Live = false;
            ComInterface.OnReceive -= OnMessageReceived;
            OnOffLoad = delegate { };
            OnReadingChanged = delegate { };
        }
    }
}