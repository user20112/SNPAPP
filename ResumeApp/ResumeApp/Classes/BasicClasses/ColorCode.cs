using System.Collections.Generic;

namespace ResumeApp.Classes
{
    public class ColorCode
    {
        public string ColorCodeName = "";
        public List<string> Cores;
        public int Count;
        public List<string> GroupingColors;
        public int Index = 0;
        public List<string> IndividualColors;
        public List<string> Sectors;

        public ColorCode(List<IndividualColorCodeSegment> colorCodes)
        {
            Cores = new List<string>();
            GroupingColors = new List<string>();
            IndividualColors = new List<string>();
            Sectors = new List<string>();
            foreach (IndividualColorCodeSegment i in colorCodes)
            {
                Cores.Add(i.Core);
                GroupingColors.Add(i.Group);
                IndividualColors.Add(i.Fiber);
                Sectors.Add(i.Sector);
            }
            Count = Cores.Count;
        }

        public ColorCode(List<string> groupingColor, List<string> individualColors, List<string> sectors, List<string> cores, string colorCodeName)
        {
            GroupingColors = groupingColor;
            IndividualColors = individualColors;
            Sectors = sectors;
            Cores = cores;
            ColorCodeName = colorCodeName;
            Count = groupingColor.Count;
        }

        public string CurrentCore
        {
            get
            {
                try { return Cores[Index]; } catch { return ""; }
            }
            set
            {
                try { Cores[Index] = value; } catch { }
            }
        }

        public string CurrentGroupingColor
        {
            get
            {
                try { return GroupingColors[Index]; } catch { return ""; }
            }
            set
            {
                try { GroupingColors[Index] = value; } catch { }
            }
        }

        public string CurrentIndividualColor
        {
            get { try { return IndividualColors[Index]; } catch { return ""; } }
            set { try { IndividualColors[Index] = value; } catch { } }
        }

        public string CurrentSector
        {
            get
            {
                try { return Sectors[Index]; } catch { return ""; }
            }
            set
            {
                try { Sectors[Index] = value; } catch { }
            }
        }

        public string GetIndex(int x)
        {
            if (x < Count)
            {
                string temp = "";
                int NumberNotNull = 0;
                if (!string.IsNullOrWhiteSpace(Sectors[x]))
                {
                    NumberNotNull++;
                    temp += Sectors[x] + "_";
                }
                if (!string.IsNullOrWhiteSpace(GroupingColors[x]))
                {
                    NumberNotNull++;
                    temp += GroupingColors[x] + "_";
                }
                if (!string.IsNullOrWhiteSpace(IndividualColors[x]))
                {
                    NumberNotNull++;
                    temp += IndividualColors[x] + "_";
                }
                if (!string.IsNullOrWhiteSpace(Cores[x]))
                {
                    NumberNotNull++;
                    temp += Cores[x];
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
            return "";
        }

        public void GoToHead()
        {
            Index = 0;
        }

        public void GoTotail()
        {
            Index = Count;
        }

        public void Next()
        {
            if (Index < Count - 1)
            {
                Index++;
            }
        }

        public void Previous()
        {
            if (Index > 0)
            {
                Index--;
            }
        }
    }
}