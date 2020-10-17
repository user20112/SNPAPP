using Newtonsoft.Json;
using ResumeApp.Interfaces;
using SkiaSharp;
using System;

namespace ResumeApp.DataClasses
{
    public class InspectionSaveData : IComparable<InspectionSaveData>
    {
        public bool AnalysisApplied = false;

        public InspectionSaveData(string sector, string cablelocation, string colorpairletter, string sxdx, string end, string comment, bool passFail, string filesavename, int fiberID, IFileSave AppSpecificFileSave, string iecSpecApplied, bool NeedsImage, bool analysisApplied)
        {
            AnalysisApplied = analysisApplied;
            FiberID = fiberID;
            Sector = sector;
            GroupingColor = cablelocation;
            IndividualColor = colorpairletter;
            CoreNumber = sxdx;
            End = end;
            Comment = comment;
            PassFail = passFail;
            FileSaveName = filesavename;
            IECSpecApplied = iecSpecApplied;
            try
            {
                if (NeedsImage)
                    Image = AppSpecificFileSave.SKImageFromFile(filesavename);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        [JsonConstructor]
        public InspectionSaveData()
        {
            Comment = "";
            CoreNumber = "";
            End = "";
            FiberID = 0;
            FileSaveName = "";
            GroupingColor = "";
            Sector = "";
            Image = null;
            IndividualColor = "";
            AnalysisApplied = false;
        }

        public string Comment { get; set; }
        public string CoreNumber { get; set; }
        public string End { get; set; }
        public int FiberID { get; set; }
        public string FileSaveName { get; set; }
        public string GroupingColor { get; set; }

        public string ID
        {
            get
            {
                string temp = "";
                int NumberNotNull = 0;
                if (!string.IsNullOrWhiteSpace(Sector))
                {
                    NumberNotNull++;
                    temp += Sector + "_";
                }
                if (!string.IsNullOrWhiteSpace(GroupingColor))
                {
                    NumberNotNull++;
                    temp += GroupingColor + "_";
                }
                if (!string.IsNullOrWhiteSpace(IndividualColor))
                {
                    NumberNotNull++;
                    temp += IndividualColor + "_";
                }
                if (!string.IsNullOrWhiteSpace(CoreNumber))
                {
                    NumberNotNull++;
                    temp += CoreNumber;
                }
                try
                {
                    if (NumberNotNull < 2)
                        temp = temp.Remove(temp.LastIndexOf('_'), 1);
                }
                catch
                {
                }
                return temp;
            }
            set
            { }
        }

        public string IECSpecApplied { get; set; } = "";

        [JsonIgnore]
        public SKImage Image { get; set; }

        public string IndividualColor { get; set; }
        public bool PassFail { get; set; }
        public SKColor PassFailColor { get { return AnalysisApplied ? PassFail ? Resources.SKColorResources.PassGreen : Resources.SKColorResources.FailRed : SKColors.Black; } }
        public string PassFailString { get { return AnalysisApplied ? PassFail ? "Pass" : "Fail" : "N/A"; } set { } }
        public string Sector { get; set; }

        public int CompareTo(InspectionSaveData other)
        {
            return FiberID.CompareTo(other.FiberID);
        }
    }
}