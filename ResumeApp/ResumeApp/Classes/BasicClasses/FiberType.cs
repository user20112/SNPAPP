namespace ResumeApp.Classes
{
    public class FiberType
    {
        public string Name;
        public IECSpec SpecToApply;
        public int StartStep;
        public int StepSize;

        public FiberType(string name, int startstep, int stepsize, IECSpec IECSpec = null)
        {
            if (IECSpec == null)
                SpecToApply = IECSpec.Defualt();
            Name = name;
            StepSize = stepsize;
            StartStep = startstep;
        }

        public static FiberType Defualt()
        {
            return new FiberType("Defualt", 25, 25);
        }
    }
}