namespace ResumeApp.Classes
{
    public class IECSpec
    {
        public int AdhesiveFailing;
        public int AfterFiveContactFailing;
        public int CladdingFailing;
        public int CladdingMajorOffset;
        public int CladdingMinorOffset;
        public int ContactFailing;
        public int ContactMajorOffset;
        public int ContactMinorOffset;
        public int CoreFailing;
        public int CoreMajorOffset;
        public int CoreMinorOffset;
        public int CorePassing;
        public string Name;

        public IECSpec(int adhesiveFailing, int claddingFailing, int contactFailing, int coreFailing, int coreMinorOffset, int coreMajorOffset, int contactMinorOffset, int contactMajorOffset, int claddingMinorOffset, int claddingMajorOffset, int after5ContactProblem, string name)
        {
            AdhesiveFailing = adhesiveFailing;
            CladdingFailing = claddingFailing;
            ContactFailing = contactFailing;
            CoreFailing = coreFailing;
            CoreMajorOffset = coreMajorOffset;
            CoreMinorOffset = coreMinorOffset;
            ContactMinorOffset = contactMinorOffset;
            ContactMajorOffset = contactMajorOffset;
            CladdingMinorOffset = claddingMinorOffset;
            CladdingMajorOffset = claddingMajorOffset;
            AfterFiveContactFailing = after5ContactProblem;
            Name = name;
        }

        public static IECSpec Defualt()
        {
            return new IECSpec(100, 5, 10, 0, 45, 55, 40, 50, 35, 50, 2, "UPC SM RL>=45 DB");
        }
    }
}