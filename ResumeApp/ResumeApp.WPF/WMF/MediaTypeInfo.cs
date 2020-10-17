using MediaFoundation;
using MediaFoundation.Alt;
using MediaFoundation.ReadWrite;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

/// <summary>
/// A class to enumerate media devices and return information based on them
/// </summary>
namespace ResumeApp.WPF.Classes.WMF
{
    /// <summary>
    /// Constructor
    /// </summary>
    public class MediaTypeInfo : ObjBase
    {
        /// <summary>
        /// Gets a list of all attributes contained in a media type and displays
        /// them as a human readable name. More or less just for practice
        ///
        /// Adapted from
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/ee663602(v=vs.85).aspx
        /// </summary>
        /// <returns>S_OK for success, nz for fail</returns>
        /// <param name="mediaTypeObj">the media type object</param>
        /// <param name="maxAttributes">the maximum number of attributes</param>
        /// <param name="outSb">The output string</param>
        public static HResult EnumerateAllAttributeNamesInMediaTypeAsText(IMFMediaType mediaTypeObj, int maxAttributes, out StringBuilder outSb)
        {
            return EnumerateAllAttributeNamesInMediaTypeAsText(mediaTypeObj, false, false, maxAttributes, out outSb);
        }

        /// <summary>
        /// Gets a list of all attributes contained in a media type and displays
        /// them as a human readable name. More or less just for practice
        ///
        /// Adapted from
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/ee663602(v=vs.85).aspx
        /// </summary>
        /// <returns>S_OK for success, nz for fail</returns>
        /// <param name="mediaTypeObj">the media type object</param>
        /// <param name="maxAttributes">the maximum number of attributes</param>
        /// <param name="outSb">The output string</param>
        /// <param name="ignoreMajorType">if true we ignore the major type attribute</param>
        /// <param name="ignoreSubType">if true we ignore the sub type attribute</param>
        public static HResult EnumerateAllAttributeNamesInMediaTypeAsText(IMFMediaType mediaTypeObj, bool ignoreMajorType, bool ignoreSubType, int maxAttributes, out StringBuilder outSb)
        {
            // we always return something here
            outSb = new StringBuilder();
            // sanity check
            if (mediaTypeObj == null) return HResult.E_FAIL;
            if ((mediaTypeObj is IMFAttributes) == false) return HResult.E_FAIL;
            // set up to ignore
            List<string> attributesToIgnore = new List<string>();
            if (ignoreMajorType == true) attributesToIgnore.Add("MF_MT_MAJOR_TYPE");
            if (ignoreSubType == true) attributesToIgnore.Add("MF_MT_SUBTYPE");
            // just call the generic WMFUtils Attribute Enumerator
            return WMFUtils.EnumerateAllAttributeNamesAsText((mediaTypeObj as IMFAttributes), attributesToIgnore, maxAttributes, out outSb);
        }

        /// <summary>
        /// Gets the major media type of a IMFMediaType as a text string
        ///
        /// Adapted from
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/ee663602(v=vs.85).aspx
        /// </summary>
        /// <returns>S_OK for success, nz for fail</returns>
        /// <param name="mediaTypeObj">the media type object</param>
        /// <param name="outSb">The output string</param>
        public static HResult GetMediaMajorTypeAsText(IMFMediaType mediaTypeObj, out StringBuilder outSb)
        {
            // we always return something here
            outSb = new StringBuilder();
            // sanity check
            if (mediaTypeObj == null) return HResult.E_FAIL;
            // MF_MT_MAJOR_TYPE
            // Major type GUID, we return this as human readable text
            HResult hr = mediaTypeObj.GetMajorType(out Guid majorType);
            if (hr == HResult.S_OK)
            {
                // only report success
                outSb.Append("MF_MT_MAJOR_TYPE=" + WMFUtils.ConvertGuidToName(majorType));
            }
            return HResult.S_OK;
        }

        /// <summary>
        /// Gets the major media type of a IMFMediaType as a text string
        ///
        /// Adapted from
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/ee663602(v=vs.85).aspx
        /// </summary>
        /// <returns>S_OK for success, nz for fail</returns>
        /// <param name="mediaTypeObj">the media type object</param>
        /// <param name="outSb">The output string</param>
        public static HResult GetMediaSubTypeAsText(IMFMediaType mediaTypeObj, out StringBuilder outSb)
        {
            // we always return something here
            outSb = new StringBuilder();
            // sanity check
            if (mediaTypeObj == null) return HResult.E_FAIL;
            // MF_MT_SUBTYPE
            // Subtype GUID which describes the basic media type, we return this as human readable text
            HResult hr = mediaTypeObj.GetGUID(MFAttributesClsid.MF_MT_SUBTYPE, out Guid subType);
            if (hr == HResult.S_OK)
            {
                // only report success
                outSb.Append("MF_MT_SUBTYPE=" + WMFUtils.ConvertGuidToName(subType));
            }
            return HResult.S_OK;
        }

        /// <summary>
        /// Gets a list of all supported video formats from a media type
        /// as a nice displayable bit of text. outSb will never be null can be
        /// empty.
        ///
        /// Adapted from
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/ee663602(v=vs.85).aspx
        /// </summary>
        /// <returns>S_OK for success, nz for fail</returns>
        /// <param name="mediaTypeObj">the media type object</param>
        /// <param name="outSb">The output string</param>
        public static HResult GetSupportedFormatsFromMediaType(IMFMediaType mediaTypeObj, out Guid majorType, out Guid subType, out int attributeCount, out int frameSizeWidth, out int frameSizeHeight, out int frameRate, out int frameRateDenominator, out int frameRateMin, out int frameRateMinDenominator, out int frameRateMax, out int frameRateMaxDenominator)
        {
            // init these
            majorType = Guid.Empty;
            subType = Guid.Empty;
            attributeCount = 0;
            frameSizeWidth = 0;
            frameSizeHeight = 0;
            frameRate = 0;
            frameRateDenominator = 0;
            frameRateMin = 0;
            frameRateMinDenominator = 0;
            frameRateMax = 0;
            frameRateMaxDenominator = 0;
            // sanity check
            if (mediaTypeObj == null) return HResult.E_FAIL;
            // Retrieves the number of attributes that are set on this object.
            HResult hr = mediaTypeObj.GetCount(out attributeCount);
            if (hr != HResult.S_OK)
            {
                // if we failed here, bail out
                return HResult.E_FAIL;
            }
            // put in this line now
            //   outSb.Append("attributeCount=" + attributeCount.ToString()+", ");
            // MF_MT_MAJOR_TYPE
            // Major type GUID, we return this as human readable text
            hr = mediaTypeObj.GetMajorType(out majorType);
            if (hr != HResult.S_OK)
            {
                // if we failed here, bail out
                return HResult.E_FAIL;
            }
            // MF_MT_SUBTYPE
            // Subtype GUID which describes the basic media type, we return this as human readable text
            hr = mediaTypeObj.GetGUID(MFAttributesClsid.MF_MT_SUBTYPE, out subType);
            if (hr != HResult.S_OK)
            {
                // if we failed here, bail out
                return HResult.E_FAIL;
            }
            // MF_MT_FRAME_SIZE
            // the Width and height of a video frame, in pixels
            hr = MFExtern.MFGetAttributeSize(mediaTypeObj, MFAttributesClsid.MF_MT_FRAME_SIZE, out frameSizeWidth, out frameSizeHeight);
            if (hr != HResult.S_OK)
            {
                // if we failed here, bail out
                return HResult.E_FAIL;
            }
            // MF_MT_FRAME_RATE
            // The frame rate is expressed as a ratio.The upper 32 bits of the attribute value contain the numerator and the lower 32 bits contain the denominator.
            // For example, if the frame rate is 30 frames per second(fps), the ratio is 30 / 1.If the frame rate is 29.97 fps, the ratio is 30,000 / 1001.
            // we report this back to the user as a decimal
            hr = MFExtern.MFGetAttributeRatio(mediaTypeObj, MFAttributesClsid.MF_MT_FRAME_RATE, out frameRate, out frameRateDenominator);
            if (hr != HResult.S_OK)
            {
                // if we failed here, bail out
                return HResult.E_FAIL;
            }
            // MF_MT_FRAME_RATE_RANGE_MIN
            // The frame rate is expressed as a ratio.The upper 32 bits of the attribute value contain the numerator and the lower 32 bits contain the denominator.
            // For example, if the frame rate is 30 frames per second(fps), the ratio is 30 / 1.If the frame rate is 29.97 fps, the ratio is 30,000 / 1001.
            // we report this back to the user as a decimal
            hr = MFExtern.MFGetAttributeRatio(mediaTypeObj, MFAttributesClsid.MF_MT_FRAME_RATE_RANGE_MIN, out frameRateMin, out frameRateMinDenominator);
            if (hr != HResult.S_OK)
            {
                // if we failed here, bail out
                return HResult.E_FAIL;
            }
            // MF_MT_FRAME_RATE_RANGE_MAX
            // The frame rate is expressed as a ratio.The upper 32 bits of the attribute value contain the numerator and the lower 32 bits contain the denominator.
            // For example, if the frame rate is 30 frames per second(fps), the ratio is 30 / 1.If the frame rate is 29.97 fps, the ratio is 30,000 / 1001.
            // we report this back to the user as a decimal
            hr = MFExtern.MFGetAttributeRatio(mediaTypeObj, MFAttributesClsid.MF_MT_FRAME_RATE_RANGE_MAX, out frameRateMax, out frameRateMaxDenominator);
            if (hr != HResult.S_OK)
            {
                // if we failed here, bail out
                return HResult.E_FAIL;
            }
            return HResult.S_OK;
        }

        /// <summary>
        /// Gets a list of all supported video formats from a media type
        /// as a nice displayable bit of text. outSb will never be null can be
        /// empty.
        ///
        /// Adapted from
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/ee663602(v=vs.85).aspx
        /// </summary>
        /// <returns>S_OK for success, nz for fail</returns>
        /// <param name="mediaTypeObj">the media type object</param>
        /// <param name="outSb">The output string</param>
        public static HResult GetSupportedFormatsFromMediaTypeAsText(IMFMediaType mediaTypeObj, out StringBuilder outSb)
        {
            // we always return something here
            outSb = new StringBuilder();
            // sanity check
            if (mediaTypeObj == null) return HResult.E_FAIL;
            // Retrieves the number of attributes that are set on this object.
            HResult hr = mediaTypeObj.GetCount(out int attributeCount);
            if (hr != HResult.S_OK)
            {
                // if we failed here, bail out
                outSb.Append("failed getting attributeCount, retVal=" + hr.ToString());
                outSb.Append("\r\n");
                return HResult.E_FAIL;
            }
            // MF_MT_MAJOR_TYPE
            // Major type GUID, we return this as human readable text
            hr = mediaTypeObj.GetMajorType(out Guid majorType);
            if (hr == HResult.S_OK)
            {
                // only report success
                outSb.Append("MF_MT_MAJOR_TYPE=" + WMFUtils.ConvertGuidToName(majorType) + ", ");
            }
            // MF_MT_SUBTYPE
            // Subtype GUID which describes the basic media type, we return this as human readable text
            hr = mediaTypeObj.GetGUID(MFAttributesClsid.MF_MT_SUBTYPE, out Guid subType);
            if (hr == HResult.S_OK)
            {
                // only report success
                outSb.Append("MF_MT_SUBTYPE=" + WMFUtils.ConvertGuidToName(subType) + ", ");
            }
            // MF_MT_FRAME_SIZE
            // the Width and height of a video frame, in pixels
            hr = MFExtern.MFGetAttributeSize(mediaTypeObj, MFAttributesClsid.MF_MT_FRAME_SIZE, out int frameSizeWidth, out int frameSizeHeight);
            if (hr == HResult.S_OK)
            {
                // only report success
                outSb.Append("MF_MT_FRAME_SIZE (W,H)=(" + frameSizeWidth.ToString() + "," + frameSizeHeight.ToString() + "), ");
            }
            // MF_MT_FRAME_RATE
            // The frame rate is expressed as a ratio.The upper 32 bits of the attribute value contain the numerator and the lower 32 bits contain the denominator.
            // For example, if the frame rate is 30 frames per second(fps), the ratio is 30 / 1.If the frame rate is 29.97 fps, the ratio is 30,000 / 1001.
            // we report this back to the user as a decimal
            hr = MFExtern.MFGetAttributeRatio(mediaTypeObj, MFAttributesClsid.MF_MT_FRAME_RATE, out int frameRate, out int frameRateDenominator);
            if (hr == HResult.S_OK)
            {
                // only report success
                if (frameRateDenominator < 0)
                {
                    outSb.Append("MF_MT_FRAME_RATE (frames/s)=(undefined),");
                }
                else
                {
                    outSb.Append("MF_MT_FRAME_RATE=" + ((decimal)frameRate / (decimal)frameRateDenominator).ToString() + "f/s, ");
                }
            }
            // MF_MT_FRAME_RATE_RANGE_MIN
            // The frame rate is expressed as a ratio.The upper 32 bits of the attribute value contain the numerator and the lower 32 bits contain the denominator.
            // For example, if the frame rate is 30 frames per second(fps), the ratio is 30 / 1.If the frame rate is 29.97 fps, the ratio is 30,000 / 1001.
            // we report this back to the user as a decimal
            hr = MFExtern.MFGetAttributeRatio(mediaTypeObj, MFAttributesClsid.MF_MT_FRAME_RATE_RANGE_MIN, out int frameRateMin, out int frameRateMinDenominator);
            if (hr == HResult.S_OK)
            {
                // only report success
                if (frameRateMinDenominator < 0)
                {
                    outSb.Append("MF_MT_FRAME_RATE_RANGE_MIN (frames/s)=(undefined),");
                }
                else
                {
                    outSb.Append("MF_MT_FRAME_RATE_RANGE_MIN=" + ((decimal)frameRateMin / (decimal)frameRateMinDenominator).ToString() + "f/s, ");
                }
            }
            // MF_MT_FRAME_RATE_RANGE_MAX
            // The frame rate is expressed as a ratio.The upper 32 bits of the attribute value contain the numerator and the lower 32 bits contain the denominator.
            // For example, if the frame rate is 30 frames per second(fps), the ratio is 30 / 1.If the frame rate is 29.97 fps, the ratio is 30,000 / 1001.
            // we report this back to the user as a decimal
            hr = MFExtern.MFGetAttributeRatio(mediaTypeObj, MFAttributesClsid.MF_MT_FRAME_RATE_RANGE_MAX, out int frameRateMax, out int frameRateMaxDenominator);
            if (hr == HResult.S_OK)
            {
                // only report success
                if (frameRateMaxDenominator < 0)
                {
                    outSb.Append("MF_MT_FRAME_RATE_RANGE_MAX (frames/s)=(undefined),");
                }
                else
                {
                    outSb.Append("MF_MT_FRAME_RATE_RANGE_MAX=" + ((decimal)frameRateMax / (decimal)frameRateMaxDenominator).ToString() + "f/s, ");
                }
            }
            // enumerate all of the possible Attributes so we can see which ones are present that we did not report on
            hr = EnumerateAllAttributeNamesInMediaTypeAsText(mediaTypeObj, attributeCount, out StringBuilder allAttrs);
            if (hr == HResult.S_OK)
            {
                outSb.Append("\r\n");
                outSb.Append("         AllAttrs=" + allAttrs.ToString());
            }
            return HResult.S_OK;
        }

        /// <summary>
        /// Gets a list of all supported video formats from a video source device
        /// as a nice displayable bit of text. outSb will never be null but can be
        /// empty. There will be one line per mediaType
        ///
        /// </summary>
        /// <returns>S_OK for success, nz for fail</returns>
        /// <param name="sourceReader">the source reader object</param>
        /// <param name="maxFormatsToTestFor">the max number of formats we test for</param>
        public static HResult GetSupportedVideoFormatsFromSourceReaderAsText(IMFSourceReader sourceReader, int maxFormatsToTestFor, out StringBuilder outSb)
        {
            IMFMediaType mediaTypeObj = null;
            HResult hr;
            // we always return something here
            outSb = new StringBuilder();
            // sanity check
            if (sourceReader == null) return HResult.E_FAIL;
            try
            {
                for (int typeIndex = 0; typeIndex < maxFormatsToTestFor; typeIndex++)
                {
                    // test this
                    hr = sourceReader.GetNativeMediaType(WMFUtils.MF_SOURCE_READER_FIRST_VIDEO_STREAM, typeIndex, out mediaTypeObj);
                    if (hr == HResult.MF_E_NO_MORE_TYPES)
                    {
                        // we are all done. The outSb container has been populated
                        return HResult.S_OK;
                    }
                    else if (hr != HResult.S_OK)
                    {
                        // we failed
                        throw new Exception("GetSupportedVideoFormatsFromSourceReaderAsText failed on call to GetNativeMediaType, retVal=" + hr.ToString());
                    }
                    // get the formats for this type
                    hr = GetSupportedFormatsFromMediaTypeAsText(mediaTypeObj, out StringBuilder tmpSb);
                    if (hr != HResult.S_OK)
                    {
                        // we failed
                        throw new Exception("GetSupportedVideoFormatsFromSourceReaderAsText failed on call to GetSupportedFormatsFromMediaTypeAsText, retVal=" + hr.ToString());
                    }
                    // add it here
                    outSb.Append(typeIndex.ToString() + " ");
                    outSb.Append(tmpSb);
                    outSb.Append("\r\n");
                    outSb.Append("\r\n");
                }
            }
            finally
            {
                // always release our mediaType if we have one
                if (mediaTypeObj != null)
                {
                    Marshal.ReleaseComObject(mediaTypeObj);
                }
            }
            // all done
            return HResult.S_OK;
        }

        /// <summary>
        /// Gets a list of all supported video formats from a video source device
        /// as a list of MFVideoFormatContainer's
        ///
        /// </summary>
        /// <returns>S_OK for success, nz for fail</returns>
        /// <param name="currentDevice">the video device that created the source reader</param>
        /// <param name="sourceReader">the source reader object</param>
        /// <param name="maxFormatsToTestFor">the max number of formats we test for</param>
        /// <param name="formatList">the list of video formats supported by the SourceReader</param>
        public static HResult GetSupportedVideoFormatsFromSourceReaderInFormatContainers(MFDevice currentDevice, IMFSourceReaderAsync sourceReader, int maxFormatsToTestFor, out List<MFVideoFormatContainer> formatList)
        {
            IMFMediaType mediaTypeObj = null;
            HResult hr;
            // init this, we never return null here
            formatList = new List<MFVideoFormatContainer>();
            // sanity check
            if (currentDevice == null) return HResult.E_FAIL;
            if (sourceReader == null) return HResult.E_FAIL;
            try
            {
                for (int typeIndex = 0; typeIndex < maxFormatsToTestFor; typeIndex++)
                {
                    // test this
                    hr = sourceReader.GetNativeMediaType(WMFUtils.MF_SOURCE_READER_FIRST_VIDEO_STREAM, typeIndex, out mediaTypeObj);
                    if (hr == HResult.MF_E_NO_MORE_TYPES)
                    {
                        // we are all done. The outSb container has been populated
                        return HResult.S_OK;
                    }
                    else if (hr != HResult.S_OK)
                    {
                        // we failed
                        throw new Exception("GetSupportedVideoFormatsFromSourceReaderInFormatContainers failed on call to GetNativeMediaType, retVal=" + hr.ToString());
                    }
                    // get a format container from the media type
                    MFVideoFormatContainer tmpContainer = GetVideoFormatContainerFromMediaTypeObject(mediaTypeObj, currentDevice);
                    if (tmpContainer == null)
                    {
                        // we failed
                        throw new Exception("GetSupportedVideoFormatsFromSourceReaderInFormatContainers failed on call to GetVideoFormatContainerFromMediaTypeObject");
                    }
                    // now add it
                    formatList.Add(tmpContainer);
                }
            }
            finally
            {
                // always release our mediaType if we have one
                if (mediaTypeObj != null)
                {
                    Marshal.ReleaseComObject(mediaTypeObj);
                }
            }
            // all done
            return HResult.S_OK;
        }

        /// <summary>
        /// Gets a MFVideoFormatContainer from an IMFMediaType
        /// </summary>
        /// <returns>S_OK for success, nz for fail</returns>
        /// <param name="mediaTypeObj">the media type object</param>
        /// <param name="currentDevice">the video device that created the source reader</param>
        public static MFVideoFormatContainer GetVideoFormatContainerFromMediaTypeObject(IMFMediaType mediaTypeObj, MFDevice currentDevice)
        {
            if (mediaTypeObj == null)
            {
                // we failed
                throw new Exception("GetVideoFormatContainerFromMediaTypeObject failedmediaTypeObj == null");
            }
            if (currentDevice == null)
            {
                // we failed
                throw new Exception("GetVideoFormatContainerFromMediaTypeObject currentDevice == null");
            }
            // get the formats for this type
            HResult hr = GetSupportedFormatsFromMediaType(mediaTypeObj, out Guid majorType, out Guid subType, out int attributeCount, out int frameSizeWidth, out int frameSizeHeight, out int frameRate, out int frameRateDenominator, out int frameRateMin, out int frameRateMinDenominator, out int frameRateMax, out int frameRateMaxDenominator);
            if (hr != HResult.S_OK)
            {
                // we failed
                throw new Exception("GetSupportedVideoFormatsFromSourceReaderInFormatContainers failed on call to GetSupportedFormatsFromMediaType, retVal=" + hr.ToString());
            }
            // enumerate all of the possible Attributes so we can see which ones are present that we did not report on
            hr = EnumerateAllAttributeNamesInMediaTypeAsText(mediaTypeObj, attributeCount, out StringBuilder allAttrs);
            if (hr != HResult.S_OK)
            {
                // we failed
                throw new Exception("GetSupportedVideoFormatsFromSourceReaderInFormatContainers failed on call to EnumerateAllAttributeNamesInMediaTypeAsText, retVal=" + hr.ToString());
            }
            // create the container here
            MFVideoFormatContainer tmpContainer = new MFVideoFormatContainer
            {
                MajorType = majorType,
                SubType = subType,
                AttributeCount = attributeCount,
                FrameSizeWidth = frameSizeWidth,
                FrameSizeHeight = frameSizeHeight,
                FrameRate = frameRate,
                FrameRateDenominator = frameRateDenominator,
                FrameRateMin = frameRateMin,
                FrameRateMinDenominator = frameRateMinDenominator,
                FrameRateMax = frameRateMax,
                FrameRateMaxDenominator = frameRateMaxDenominator,
                AllAttributes = allAttrs.ToString(),
                VideoDevice = currentDevice
            };
            return tmpContainer;
        }
    }
}