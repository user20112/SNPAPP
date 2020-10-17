namespace ResumeApp.Interfaces
{
    public interface IFocusInterface
    {
        int AvgFrameLatency { get; set; }
        int CurrentStep { get; set; }
        int MaxFrameLatency { get; set; }

        void Display(string StringToDisplay);

        void ResetFrameInfo();

        void StepBackward(int Steps, ref bool keepFocusing);

        void StepForward(int Steps, ref bool keepFocusing);

        void StepHome();

        int WaitTillUpdated(int MaxWait, ref bool keepFocusing, int Trigger, int TriggerWait);
    }
}