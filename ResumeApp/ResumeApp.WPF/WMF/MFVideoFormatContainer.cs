using System;

namespace ResumeApp.WPF.Classes.WMF
{
    /// <summary>
    /// A class to contain Video Format Information
    ///
    /// </summary>
    public class MFVideoFormatContainer : ObjBase, IComparable
    {
        private string allAttributes = "";
        private int attributeCount = 0;
        private int frameRate = 0;
        private int frameRateDenominator = 0;
        private int frameRateMax = 0;
        private int frameRateMaxDenominator = 0;
        private int frameRateMin = 0;
        private int frameRateMinDenominator = 0;
        private int frameSizeHeight = 0;
        private int frameSizeWidth = 0;
        private Guid majorType = Guid.Empty;
        private Guid subType = Guid.Empty;
        private MFDevice videoDevice = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public MFVideoFormatContainer()
        {
        }

        /// <summary>
        /// Gets/sets the allAttributes as a string. Never gets/sets null.
        /// </summary>
        public string AllAttributes
        {
            get
            {
                if (allAttributes == null) allAttributes = "";
                return allAttributes;
            }
            set
            {
                allAttributes = value;
                if (allAttributes == null) allAttributes = "";
            }
        }

        public int AttributeCount { get { return attributeCount; } set { attributeCount = value; } }
        public int FrameRate { get { return frameRate; } set { frameRate = value; } }

        /// <summary>
        /// Gets the FrameRate as a string
        /// </summary>
        public string FrameRateAsString
        {
            get
            {
                if (frameRateDenominator < 0)
                {
                    return "(undefined)";
                }
                else
                {
                    return Math.Round(((decimal)frameRate / (decimal)frameRateDenominator)).ToString();
                }
            }
        }

        public int FrameRateDenominator { get { return frameRateDenominator; } set { frameRateDenominator = value; } }
        public int FrameRateMax { get { return frameRateMax; } set { frameRateMax = value; } }

        /// <summary>
        /// Gets the FrameRateMax as a string
        /// </summary>
        public string FrameRateMaxAsString
        {
            get
            {
                if (frameRateMaxDenominator < 0)
                {
                    return "(undefined)";
                }
                else
                {
                    return Math.Round(((decimal)frameRateMax / (decimal)frameRateMaxDenominator)).ToString();
                }
            }
        }

        public int FrameRateMaxDenominator { get { return frameRateMaxDenominator; } set { frameRateMaxDenominator = value; } }
        public int FrameRateMin { get { return frameRateMin; } set { frameRateMin = value; } }

        /// <summary>
        /// Gets the FrameRateMin as a string
        /// </summary>
        public string FrameRateMinAsString
        {
            get
            {
                if (frameRateMinDenominator < 0)
                {
                    return "(undefined)";
                }
                else
                {
                    return Math.Round(((decimal)frameRateMin / (decimal)frameRateMinDenominator)).ToString();
                }
            }
        }

        public int FrameRateMinDenominator { get { return frameRateMinDenominator; } set { frameRateMinDenominator = value; } }

        /// <summary>
        /// Gets the FrameSize as a string
        /// </summary>
        public string FrameSizeAsString
        {
            get
            {
                return "(" + frameSizeWidth.ToString() + "," + frameSizeHeight.ToString() + ")";
            }
        }

        public int FrameSizeHeight { get { return frameSizeHeight; } set { frameSizeHeight = value; } }
        public int FrameSizeWidth { get { return frameSizeWidth; } set { frameSizeWidth = value; } }

        // get and set accessors
        public Guid MajorType { get { return majorType; } set { majorType = value; } }

        public Guid SubType { get { return subType; } set { subType = value; } }

        /// <summary>
        /// Gets the SubType as a string
        /// </summary>
        public string SubTypeAsString
        {
            get
            {
                return WMFUtils.ConvertGuidToName(SubType);
            }
        }

        /// <summary>
        /// Gets/sets the video device which owns these formats. Will get/set null.
        /// </summary>
        public MFDevice VideoDevice
        {
            get
            {
                return videoDevice;
            }
            set
            {
                videoDevice = value;
            }
        }

        /// <summary>
        /// IComparable implementation
        /// </summary>
        /// <param name="obj">Our container to compare to</param>
        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            if ((obj is MFVideoFormatContainer) == false) return 2;
            if (MajorType != (obj as MFVideoFormatContainer).MajorType) return 10;
            if (SubType != (obj as MFVideoFormatContainer).SubType) return 11;
            if (AttributeCount != (obj as MFVideoFormatContainer).AttributeCount) return 12;
            if (FrameSizeWidth != (obj as MFVideoFormatContainer).FrameSizeWidth) return 13;
            if (FrameSizeHeight != (obj as MFVideoFormatContainer).FrameSizeHeight) return 14;
            if (FrameRate != (obj as MFVideoFormatContainer).FrameRate) return 15;
            if (FrameRateDenominator != (obj as MFVideoFormatContainer).FrameRateDenominator) return 16;
            if (FrameRateMin != (obj as MFVideoFormatContainer).FrameRateMin) return 17;
            if (FrameRateMinDenominator != (obj as MFVideoFormatContainer).FrameRateMinDenominator) return 18;
            if (FrameRateMax != (obj as MFVideoFormatContainer).FrameRateMax) return 19;
            if (FrameRateMaxDenominator != (obj as MFVideoFormatContainer).FrameRateMaxDenominator) return 20;
            if (AllAttributes != (obj as MFVideoFormatContainer).AllAttributes) return 21;
            // now compare the video devices
            if ((this.VideoDevice == null) && ((obj as MFVideoFormatContainer).VideoDevice == null)) return 0;
            if ((this.VideoDevice == null) && ((obj as MFVideoFormatContainer).VideoDevice != null)) return 30;
            if ((this.VideoDevice != null) && ((obj as MFVideoFormatContainer).VideoDevice == null)) return 40;
            // compare the two devices
            return this.VideoDevice.CompareTo((obj as MFVideoFormatContainer).VideoDevice);
        }

        /// <summary>
        /// A display summary string
        /// </summary>
        public string DisplayString()
        {
            return SubTypeAsString + " " + FrameSizeAsString;
        }

        /// <summary>
        /// Override the ToString()
        /// </summary>
        public override string ToString()
        {
            // just provide the subtype - we already know it is of type video
            return SubTypeAsString;
        }
    }
}