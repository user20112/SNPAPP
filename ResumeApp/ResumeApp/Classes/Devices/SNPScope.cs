using ResumeApp.Events;
using ResumeApp.Interfaces;
using System;
using System.Threading;

namespace ResumeApp.Classes.Devices
{
    internal class SNPScope : Scope
    {
        public ICommunicationInterface ComInterface;

        public SNPScope(ICommunicationInterface comInterface)
        {
            ComInterface = comInterface;
            ComInterface.OnReceive += DataReceived;
        }

        private string buffer = "";

        private void DataReceived(object sender, string e)
        {
            buffer += e;
            if (buffer.Contains("StartImage"))
            {
                if (buffer.Contains("EndImage"))
                {
                    int x = buffer.IndexOf("StartImage");
                    int y = buffer.IndexOf("EndImage");
                    HandleFrame(buffer.Substring(x + 10, y - x - 10));
                    buffer = "";
                    return;
                }
            }
        }

        private void HandleFrame(string v)
        {
            //SkiaSharp.SKImage received = SkiaSharp.SKImage.FromEncodedData();
        }

        public override void AfterAnalysis(bool Passed, bool Completed)
        {
        }

        public override void Center(bool Toggle, EventHandler<DisplayEventArgs> Display, SynchronizationContext Main)
        {
        }

        public override void Dispose()
        {
        }

        public override void PauseStream(bool toggle)
        {
        }

        public override void ReCenter(EventHandler<DisplayEventArgs> Display, SynchronizationContext Main)
        {
        }

        public override void StartStream(EventHandler<FrameReadyEventArgs> OnFrame, SynchronizationContext Context, EventHandler<EventArgs> ButtonPressed)
        {
        }

        public override void StopStream()
        {
        }
    }
}