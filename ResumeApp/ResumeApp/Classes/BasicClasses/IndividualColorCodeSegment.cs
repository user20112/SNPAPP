using System;
using System.Collections.Generic;

namespace ResumeApp.Classes
{
    public class IndividualColorCodeSegment : IComparable<IndividualColorCodeSegment>
    {
        public IndividualColorCodeSegment(string core, string fiber, int fiberID, string group, string sector)
        {
            Core = core;
            Fiber = fiber;
            Group = group;
            Sector = sector;
            FiberID = fiberID;
        }

        public string Core { get; set; }
        public string Fiber { get; set; }
        public int FiberID { get; set; }
        public string Group { get; set; }

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
                if (!string.IsNullOrWhiteSpace(Group))
                {
                    NumberNotNull++;
                    temp += Group + "_";
                }
                if (!string.IsNullOrWhiteSpace(Fiber))
                {
                    NumberNotNull++;
                    temp += Fiber + "_";
                }
                if (!string.IsNullOrWhiteSpace(Core))
                {
                    NumberNotNull++;
                    temp += Core;
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
            set { }
        }

        public string Sector { get; set; }

        public static List<IndividualColorCodeSegment> GetIndividualColorCodeSegments(ColorCode Code)
        {
            List<IndividualColorCodeSegment> ReturnData = new List<IndividualColorCodeSegment>();
            for (int x = 0; x < Code.Count; x++)
            {
                IndividualColorCodeSegment colorCodeSegment = new IndividualColorCodeSegment(Code.Cores[x], Code.IndividualColors[x], x, Code.GroupingColors[x], Code.Sectors[x]);
                ReturnData.Add(colorCodeSegment);
            }
            return ReturnData;
        }

        public int CompareTo(IndividualColorCodeSegment other)
        {
            return FiberID.CompareTo(other.FiberID);
        }
    }
}