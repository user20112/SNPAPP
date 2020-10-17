using Newtonsoft.Json;
using SkiaSharp;
using System;

namespace ResumeApp.Classes
{
    public class Reading : IComparable
    {
        public bool ColorEnabled = false;
        public double Reference = 0;
        public DateTime TimeMeasured;

        public Reading(string measurement, bool passFail, string unit, string wavelength, string id, DateTime date, int index, double reference, string toneDetect)
        {
            Measurement = measurement;
            PassFail = passFail;
            ColorEnabled = true;
            Unit = unit;
            Wavelength = wavelength;
            ID = id;
            TimeMeasured = date;
            Index = index;
            Reference = reference;
            ToneDetected = toneDetect;
            if (ToneDetected == "")
                ToneDetected = "None";
        }

        public Reading(string measurement, string unit, string wavelength, string id, double reference, DateTime TimeMeasured, string toneDetect)
        {
            Measurement = measurement;
            PassFail = false;
            Unit = unit;
            Wavelength = wavelength;
            ID = id;
            Index = 0;
            Reference = reference;
            ToneDetected = toneDetect;
            this.TimeMeasured = TimeMeasured;
            if (ToneDetected == "")
                ToneDetected = "None";
        }

        [JsonConstructor]
        public Reading()
        {
            Measurement = "";
            PassFail = false;
            Unit = "";
            Wavelength = "";
            ID = "";
            Index = 0;
            TimeMeasured = DateTime.Now;
            ToneDetected = "None";
        }

        public string Date { get; set; }

        public string DisplayValue { get { if (Unit != null) return Measurement + " " + Unit; else return Measurement; } set { } }

        public string DisplayWavelength { get { if (Wavelength != "No W") return "λ " + Wavelength + "nm"; else return ""; } set { } }

        public string ID { get; set; }

        public int Index { get; set; }

        public string Measurement { get; set; }

        public bool PassFail { get; set; }

        public SKColor PassFailColor { get { if (ColorEnabled) { if (PassFail) return Resources.SKColorResources.PassGreen; else return Resources.SKColorResources.FailRed; } return SKColors.Black; } }

        public string PassFailString { get { return ColorEnabled ? PassFail ? "Pass" : "Fail" : "N/A"; } set { } }

        public string ToneDetected { get; set; } = "None";

        public string Unit { get; set; }

        public string Wavelength { get; set; }

        public int CompareTo(object obj)
        {
            int temp = Index - (((Reading)obj).Index);
            if (temp == 0)
            {
                try
                {
                    return Convert.ToInt32(Wavelength) - Convert.ToInt32(((Reading)obj).Wavelength);
                }
                catch
                {
                    return temp;
                }
            }
            return temp;
        }
    }
}