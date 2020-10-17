using MediaFoundation;
using MediaFoundation.Alt;
using ResumeApp.Classes;
using ResumeApp.Events;
using ResumeApp.Interfaces;
using ResumeApp.WPF.Classes.WMF;
using ResumeApp.WPF.InterfaceImplimentations;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(MFVideoInterface))]

namespace ResumeApp.WPF.InterfaceImplimentations
{
    public class MFVideoInterface : IVideoSource
    {
        public new bool ButtonAvailable = true;
        private int IMFSnapIndex = 0;
        private int IMFStreamIndex = 0;
        private MFDevice UnderlyingDevice;
        private IMFMediaType currentVideoMediaType;
        private IMFMediaSession mediaSession;
        private AsyncCallbackHandler mediaSessionAsyncCallbackHandler;
        private IMFMediaSource MediaSource;
        private IMFSourceReaderAsync StreamReader;
        private StreamHandler StreamReaderHandler = null;

        public MFVideoInterface()
        {
        }

        public override void Dispose()
        {
            CloseAllMediaDevices();
        }

        public override void PauseStream(bool Toggle)
        {
            Paused = Toggle;
        }

        public override void StartStream()
        {
            Task.Run(() =>
            {
                // check our source filename is correct and usable
                StreamReaderHandler = new StreamHandler(ProcessFrame, MFBPressed);
                HResult hr;
                IMFSourceResolver pSourceResolver = null;
                IMFPresentationDescriptor sourcePresentationDescriptor = null;
                IMFStreamDescriptor videoStreamDescriptor = null;
                try
                {
                    // reset everything
                    CloseAllMediaDevices();
                    // Create the media session.
                    hr = MFExtern.MFCreateMediaSession(null, out mediaSession);
                    if (hr != HResult.S_OK)
                    {
                        throw new Exception("PrepareSessionAndTopology call to MFExtern.MFCreateMediaSession failed. Err=" + hr.ToString());
                    }
                    // set up our media session call back handler.
                    mediaSessionAsyncCallbackHandler = new AsyncCallbackHandler();
                    mediaSessionAsyncCallbackHandler.Initialize();
                    mediaSessionAsyncCallbackHandler.MediaSession = mediaSession;
                    mediaSessionAsyncCallbackHandler.MediaSessionAsyncCallBackError = HandleMediaSessionAsyncCallBackErrors;
                    mediaSessionAsyncCallbackHandler.MediaSessionAsyncCallBackEvent = HandleMediaSessionAsyncCallBackEvent;
                    // Register the callback handler with the session and tell it that events can
                    // start. This does not actually trigger an event it just lets the media session
                    // know that it can now send them if it wishes to do so.
                    hr = mediaSession.BeginGetEvent(mediaSessionAsyncCallbackHandler, null);
                    if (hr != HResult.S_OK)
                    {
                        throw new Exception("PrepareSessionAndTopology call to mediaSession.BeginGetEvent failed. Err=" + hr.ToString());
                    }
                    StreamReader = WMFUtils.CreateSourceReaderAsyncFromDevice(UnderlyingDevice, StreamReaderHandler);
                    try
                    {
                        SetCurrentMediaType(StreamReader, IMFStreamIndex, 0);
                    }
                    catch
                    {
                    }
                    try
                    {
                        SetCurrentMediaType(StreamReader, IMFSnapIndex, 1);
                    }
                    catch
                    {
                    }
                    StreamReaderHandler.StreamReader = StreamReader;
                    StreamReader.GetCurrentMediaType(1, out IMFMediaType ppMediaType);
                    StreamReader.GetNativeMediaType(1, 0, out IMFMediaType PPMediaType);
                    StreamReader.GetCurrentMediaType(0, out IMFMediaType ppMediaType1);
                    StreamReader.GetNativeMediaType(0, 0, out IMFMediaType PPMediaType1);
                    ppMediaType.GetMajorType(out Guid ppMediaTypeMGUID);
                    PPMediaType.GetMajorType(out Guid PPMediaTypeMGUID);
                    ppMediaType.GetGUID(MFAttributesClsid.MF_MT_SUBTYPE, out Guid ppMediaTypeSGUID);
                    PPMediaType.GetGUID(MFAttributesClsid.MF_MT_SUBTYPE, out Guid PPMediaTypeSGUID);
                    Console.WriteLine(WMFUtils.ConvertGuidToName(ppMediaTypeMGUID));
                    Console.WriteLine(WMFUtils.ConvertGuidToName(PPMediaTypeMGUID));
                    Console.WriteLine(WMFUtils.ConvertGuidToName(ppMediaTypeSGUID));
                    Console.WriteLine(WMFUtils.ConvertGuidToName(PPMediaTypeSGUID));
                    ppMediaType1.GetMajorType(out Guid ppMediaTypeMGUID1);
                    PPMediaType1.GetMajorType(out Guid PPMediaTypeMGUID1);
                    ppMediaType1.GetGUID(MFAttributesClsid.MF_MT_SUBTYPE, out Guid ppMediaTypeSGUID1);
                    PPMediaType1.GetGUID(MFAttributesClsid.MF_MT_SUBTYPE, out Guid PPMediaTypeSGUID1);
                    Console.WriteLine(WMFUtils.ConvertGuidToName(ppMediaTypeMGUID1));
                    Console.WriteLine(WMFUtils.ConvertGuidToName(PPMediaTypeMGUID1));
                    Console.WriteLine(WMFUtils.ConvertGuidToName(ppMediaTypeSGUID1));
                    Console.WriteLine(WMFUtils.ConvertGuidToName(PPMediaTypeSGUID1));
                    Paused = false;
                    StreamReader.SetStreamSelection(0, true);
                    StreamReader.SetStreamSelection(1, true);
                    hr = StreamReader.ReadSample(
                            0,
                            MediaFoundation.ReadWrite.MF_SOURCE_READER_CONTROL_FLAG.None,
                            IntPtr.Zero,
                            IntPtr.Zero,
                            IntPtr.Zero,
                            IntPtr.Zero
                            );
                    hr = StreamReader.ReadSample(1,
                            MediaFoundation.ReadWrite.MF_SOURCE_READER_CONTROL_FLAG.None,
                            IntPtr.Zero,
                            IntPtr.Zero,
                            IntPtr.Zero,
                            IntPtr.Zero
                            );
                    if (hr != HResult.S_OK)
                    {
                        // we failed
                        throw new Exception("Failed on calling the first ReadSample on the reader, retVal=" + hr.ToString());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("PrepareSessionAndTopology Error: " + ex.Message);
                }
                finally
                {
                    // Clean up
                    if (pSourceResolver != null)
                    {
                        Marshal.ReleaseComObject(pSourceResolver);
                    }
                    if (sourcePresentationDescriptor != null)
                    {
                        Marshal.ReleaseComObject(sourcePresentationDescriptor);
                    }
                    if (videoStreamDescriptor != null)
                    {
                        Marshal.ReleaseComObject(videoStreamDescriptor);
                    }
                }
            });
        }

        public override void StopStream()
        {
            CloseAllMediaDevices();
        }

        private void CloseAllMediaDevices()
        {
            HResult hr;
            Console.WriteLine("CloseAllMediaDevices");
            // close and release our call back handler
            if (mediaSessionAsyncCallbackHandler != null)
            {
                // stop any messaging or events in the call back handler
                mediaSessionAsyncCallbackHandler.ShutDown();
                mediaSessionAsyncCallbackHandler = null;
            }
            // Shut down the source reader
            if (StreamReader != null)
            {
                Marshal.ReleaseComObject(StreamReader);
                StreamReader = null;
            }
            // close the session (this is NOT the same as shutting it down)
            if (mediaSession != null)
            {
                hr = mediaSession.Close();
                if (hr != HResult.S_OK)
                {
                    // just log it
                    Console.WriteLine("CloseAllMediaDevices call to mediaSession.Close failed. Err=" + hr.ToString());
                }
            }
            // Shut down the media source
            if (MediaSource != null)
            {
                hr = MediaSource.Shutdown();
                if (hr != HResult.S_OK)
                {
                    // just log it
                    Console.WriteLine("CloseAllMediaDevices call to mediaSource.Shutdown failed. Err=" + hr.ToString());
                }
                Marshal.ReleaseComObject(MediaSource);
                MediaSource = null;
            }
            // Shut down the media session (note we only closed it before).
            if (mediaSession != null)
            {
                hr = mediaSession.Shutdown();
                if (hr != HResult.S_OK)
                {
                    // just log it
                    Console.WriteLine("CloseAllMediaDevices call to mediaSession.Shutdown failed. Err=" + hr.ToString());
                }
                Marshal.ReleaseComObject(mediaSession);
                mediaSession = null;
            }
            if (currentVideoMediaType != null)
            {
                Marshal.ReleaseComObject(currentVideoMediaType);
                currentVideoMediaType = null;
            }
        }

        private int GetRequestedSnapFormat(int WidthRequested, int HeightRequested, string EncodingRequested)
        {
            int x = 0;
            List<MFVideoFormatContainer> Formats = GetSupportedFormats(1);
            foreach (MFVideoFormatContainer format in Formats)
            {
                if (format.FrameSizeWidth == WidthRequested)
                {
                    if (format.FrameSizeHeight == HeightRequested)
                    {
                        if (format.SubTypeAsString == EncodingRequested)
                        {
                            Console.WriteLine("Requested Format: " + format.SubTypeAsString + "index:" + x);
                            break;
                        }
                    }
                }
                x++;
            }
            return x;
        }

        private int GetRequestedStreamFormat(int FrameRateRequested, int WidthRequested, int HeightRequested, string EncodingRequested)
        {
            int x = 0;
            List<MFVideoFormatContainer> Formats = GetSupportedFormats(0);
            foreach (MFVideoFormatContainer format in Formats)
            {
                if (format.AllAttributes.Contains("MF_CAPTURE"))
                {
                    if (format.AllAttributes.Contains("MF_CAPTURE_ENGINE_STREAM_CATEGORY"))
                    {
                    }
                }
                if (format.FrameRate == FrameRateRequested)
                {
                    if (format.FrameSizeWidth == WidthRequested)
                    {
                        if (format.FrameSizeHeight == HeightRequested)
                        {
                            if (format.SubTypeAsString == EncodingRequested)
                            {
                                break;
                            }
                        }
                    }
                }
                x++;
            }
            return x;
        }

        private List<MFVideoFormatContainer> GetSupportedFormats(int SourceIndex)
        {
            if (UnderlyingDevice != null)
            {
                IMFPresentationDescriptor sourcePresentationDescriptor = null;
                IMFStreamDescriptor videoStreamDescriptor = null;
                IMFMediaTypeHandler typeHandler = null;
                List<MFVideoFormatContainer> formatList = new List<MFVideoFormatContainer>();
                HResult hr;
                IMFMediaSource mediaSource = null;
                try
                {
                    // use the device symbolic name to create the media source for the video device. Media sources are objects that generate media data.
                    // For example, the data might come from a video file, a network stream, or a hardware device, such as a camera. Each
                    // media source contains one or more streams, and each stream delivers data of one type, such as audio or video.
                    mediaSource = WMFUtils.GetMediaSourceFromDevice(UnderlyingDevice);
                    if (mediaSource == null)
                    {
                        throw new Exception("DisplayVideoFormatsForCurrentCaptureDevice call to mediaSource == null");
                    }
                    // A presentation is a set of related media streams that share a common presentation time.
                    // we don't need that functionality in this app but we do need to presentation descriptor
                    // to find out the stream descriptors, these will give us the media types on offer
                    hr = mediaSource.CreatePresentationDescriptor(out sourcePresentationDescriptor);
                    if (hr != HResult.S_OK)
                    {
                        throw new Exception("DisplayVideoFormatsForCurrentCaptureDevice call to mediaSource.CreatePresentationDescriptor failed. Err=" + hr.ToString());
                    }
                    if (sourcePresentationDescriptor == null)
                    {
                        throw new Exception("DisplayVideoFormatsForCurrentCaptureDevice call to mediaSource.CreatePresentationDescriptor failed. sourcePresentationDescriptor == null");
                    }
                    // Now we get the number of stream descriptors in the presentation.
                    // A presentation descriptor contains a list of one or more
                    // stream descriptors.
                    hr = sourcePresentationDescriptor.GetStreamDescriptorCount(out int sourceStreamCount);
                    if (hr != HResult.S_OK)
                    {
                        throw new Exception("DisplayVideoFormatsForCurrentCaptureDevice call to sourcePresentationDescriptor.GetStreamDescriptorCount failed. Err=" + hr.ToString());
                    }
                    if (sourceStreamCount == 0)
                    {
                        throw new Exception("DisplayVideoFormatsForCurrentCaptureDevice call to sourcePresentationDescriptor.GetStreamDescriptorCount failed. sourceStreamCount == 0");
                    }
                    // look for the video stream
                    // we require the major type to be video
                    Guid guidMajorType = WMFUtils.GetMajorMediaTypeFromPresentationDescriptor(sourcePresentationDescriptor, SourceIndex);
                    if (guidMajorType != MFMediaType.Video)
                        return new List<MFVideoFormatContainer>();
                    // we also require the stream to be enabled
                    sourcePresentationDescriptor.SelectStream(1);
                    hr = sourcePresentationDescriptor.GetStreamDescriptorByIndex(SourceIndex, out bool streamIsSelected, out videoStreamDescriptor);
                    if (hr != HResult.S_OK)
                    {
                        throw new Exception("DisplayVideoFormatsForCurrentCaptureDevice call to sourcePresentationDescriptor.GetStreamDescriptorByIndex(v) failed. Err=" + hr.ToString());
                    }
                    if (videoStreamDescriptor == null)
                    {
                        throw new Exception("DisplayVideoFormatsForCurrentCaptureDevice call to sourcePresentationDescriptor.GetStreamDescriptorByIndex(v) failed. videoStreamDescriptor == null");
                    }
                    // if the stream is not selected (enabled) look for the next
                    if (streamIsSelected == false)
                    {
                        Marshal.ReleaseComObject(videoStreamDescriptor);
                        videoStreamDescriptor = null;
                        return new List<MFVideoFormatContainer>();
                    }
                    // Get the media type handler for the stream. IMFMediaTypeHandler
                    // interface is a standard way of looking at the media types on an stream
                    hr = videoStreamDescriptor.GetMediaTypeHandler(out typeHandler);
                    if (hr != HResult.S_OK)
                    {
                        throw new Exception("call to videoStreamDescriptor.GetMediaTypeHandler failed. Err=" + hr.ToString());
                    }
                    if (typeHandler == null)
                    {
                        throw new Exception("call to videoStreamDescriptor.GetMediaTypeHandler failed. typeHandler == null");
                    }
                    // Now we get the number of media types in the stream descriptor.
                    hr = typeHandler.GetMediaTypeCount(out int mediaTypeCount);
                    if (hr != HResult.S_OK)
                    {
                        throw new Exception("DisplayVideoFormatsForCurrentCaptureDevice call to typeHandler.GetMediaTypeCount failed. Err=" + hr.ToString());
                    }
                    if (mediaTypeCount == 0)
                    {
                        throw new Exception("DisplayVideoFormatsForCurrentCaptureDevice call to typeHandler.GetMediaTypeCount failed. mediaTypeCount == 0");
                    }
                    // now loop through each media type
                    for (int mediaTypeId = 0; mediaTypeId < mediaTypeCount; mediaTypeId++)
                    {
                        // Now we have the handler, get the media type.
                        hr = typeHandler.GetMediaTypeByIndex(mediaTypeId, out IMFMediaType workingMediaType);
                        if (hr != HResult.S_OK)
                        {
                            throw new Exception("GetMediaTypeFromStreamDescriptorById call to typeHandler.GetMediaTypeByIndex failed. Err=" + hr.ToString());
                        }
                        if (workingMediaType == null)
                        {
                            throw new Exception("GetMediaTypeFromStreamDescriptorById call to typeHandler.GetMediaTypeByIndex failed. workingMediaType == null");
                        }
                        MFVideoFormatContainer tmpContainer = MediaTypeInfo.GetVideoFormatContainerFromMediaTypeObject(workingMediaType, UnderlyingDevice);
                        if (tmpContainer == null)
                        {
                            // we failed
                            throw new Exception("GetSupportedVideoFormatsFromSourceReaderInFormatContainers failed on call to GetVideoFormatContainerFromMediaTypeObject");
                        }
                        // now add it
                        formatList.Add(tmpContainer);
                        Marshal.ReleaseComObject(workingMediaType);
                        workingMediaType = null;
                    }
                    return formatList;
                }
                finally
                {
                    // close and release
                    if (mediaSource != null)
                    {
                        Marshal.ReleaseComObject(mediaSource);
                    }
                    if (sourcePresentationDescriptor != null)
                    {
                        Marshal.ReleaseComObject(sourcePresentationDescriptor);
                    }
                    if (videoStreamDescriptor != null)
                    {
                        Marshal.ReleaseComObject(videoStreamDescriptor);
                    }
                    if (typeHandler != null)
                    {
                        Marshal.ReleaseComObject(typeHandler);
                    }
                }
            }
            return new List<MFVideoFormatContainer>();
        }

        private List<MFVideoFormatContainer> GetSupportedFormats(int SourceIndex, string FriendlyName)
        {
            MFDevice UnderlyingDevice = null;
            List<MFDevice> vcDevices = WMFUtils.GetDevicesByCategory(MFAttributesClsid.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE, CLSID.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_GUID);
            foreach (MFDevice device in vcDevices)
            {
                if (device.FriendlyName == FriendlyName)
                {
                    UnderlyingDevice = device;
                    break;
                }
            }
            if (UnderlyingDevice != null)
            {
                IMFPresentationDescriptor sourcePresentationDescriptor = null;
                IMFStreamDescriptor videoStreamDescriptor = null;
                IMFMediaTypeHandler typeHandler = null;
                List<MFVideoFormatContainer> formatList = new List<MFVideoFormatContainer>();
                HResult hr;
                IMFMediaSource mediaSource = null;
                try
                {
                    // use the device symbolic name to create the media source for the video device. Media sources are objects that generate media data.
                    // For example, the data might come from a video file, a network stream, or a hardware device, such as a camera. Each
                    // media source contains one or more streams, and each stream delivers data of one type, such as audio or video.
                    mediaSource = WMFUtils.GetMediaSourceFromDevice(UnderlyingDevice);
                    if (mediaSource == null)
                    {
                        throw new Exception("DisplayVideoFormatsForCurrentCaptureDevice call to mediaSource == null");
                    }
                    // A presentation is a set of related media streams that share a common presentation time.
                    // we don't need that functionality in this app but we do need to presentation descriptor
                    // to find out the stream descriptors, these will give us the media types on offer
                    hr = mediaSource.CreatePresentationDescriptor(out sourcePresentationDescriptor);
                    if (hr != HResult.S_OK)
                    {
                        throw new Exception("DisplayVideoFormatsForCurrentCaptureDevice call to mediaSource.CreatePresentationDescriptor failed. Err=" + hr.ToString());
                    }
                    if (sourcePresentationDescriptor == null)
                    {
                        throw new Exception("DisplayVideoFormatsForCurrentCaptureDevice call to mediaSource.CreatePresentationDescriptor failed. sourcePresentationDescriptor == null");
                    }
                    // Now we get the number of stream descriptors in the presentation.
                    // A presentation descriptor contains a list of one or more
                    // stream descriptors.
                    hr = sourcePresentationDescriptor.GetStreamDescriptorCount(out int sourceStreamCount);
                    if (hr != HResult.S_OK)
                    {
                        throw new Exception("DisplayVideoFormatsForCurrentCaptureDevice call to sourcePresentationDescriptor.GetStreamDescriptorCount failed. Err=" + hr.ToString());
                    }
                    if (sourceStreamCount == 0)
                    {
                        throw new Exception("DisplayVideoFormatsForCurrentCaptureDevice call to sourcePresentationDescriptor.GetStreamDescriptorCount failed. sourceStreamCount == 0");
                    }
                    // look for the video stream
                    // we require the major type to be video
                    Guid guidMajorType = WMFUtils.GetMajorMediaTypeFromPresentationDescriptor(sourcePresentationDescriptor, SourceIndex);
                    if (guidMajorType != MFMediaType.Video)
                        return new List<MFVideoFormatContainer>();
                    // we also require the stream to be enabled
                    sourcePresentationDescriptor.SelectStream(1);
                    hr = sourcePresentationDescriptor.GetStreamDescriptorByIndex(SourceIndex, out bool streamIsSelected, out videoStreamDescriptor);
                    if (hr != HResult.S_OK)
                    {
                        throw new Exception("DisplayVideoFormatsForCurrentCaptureDevice call to sourcePresentationDescriptor.GetStreamDescriptorByIndex(v) failed. Err=" + hr.ToString());
                    }
                    if (videoStreamDescriptor == null)
                    {
                        throw new Exception("DisplayVideoFormatsForCurrentCaptureDevice call to sourcePresentationDescriptor.GetStreamDescriptorByIndex(v) failed. videoStreamDescriptor == null");
                    }
                    // if the stream is not selected (enabled) look for the next
                    if (streamIsSelected == false)
                    {
                        Marshal.ReleaseComObject(videoStreamDescriptor);
                        videoStreamDescriptor = null;
                        return new List<MFVideoFormatContainer>();
                    }
                    // Get the media type handler for the stream. IMFMediaTypeHandler
                    // interface is a standard way of looking at the media types on an stream
                    hr = videoStreamDescriptor.GetMediaTypeHandler(out typeHandler);
                    if (hr != HResult.S_OK)
                    {
                        throw new Exception("call to videoStreamDescriptor.GetMediaTypeHandler failed. Err=" + hr.ToString());
                    }
                    if (typeHandler == null)
                    {
                        throw new Exception("call to videoStreamDescriptor.GetMediaTypeHandler failed. typeHandler == null");
                    }
                    // Now we get the number of media types in the stream descriptor.
                    hr = typeHandler.GetMediaTypeCount(out int mediaTypeCount);
                    if (hr != HResult.S_OK)
                    {
                        throw new Exception("DisplayVideoFormatsForCurrentCaptureDevice call to typeHandler.GetMediaTypeCount failed. Err=" + hr.ToString());
                    }
                    if (mediaTypeCount == 0)
                    {
                        throw new Exception("DisplayVideoFormatsForCurrentCaptureDevice call to typeHandler.GetMediaTypeCount failed. mediaTypeCount == 0");
                    }
                    // now loop through each media type
                    for (int mediaTypeId = 0; mediaTypeId < mediaTypeCount; mediaTypeId++)
                    {
                        // Now we have the handler, get the media type.
                        hr = typeHandler.GetMediaTypeByIndex(mediaTypeId, out IMFMediaType workingMediaType);
                        if (hr != HResult.S_OK)
                        {
                            throw new Exception("GetMediaTypeFromStreamDescriptorById call to typeHandler.GetMediaTypeByIndex failed. Err=" + hr.ToString());
                        }
                        if (workingMediaType == null)
                        {
                            throw new Exception("GetMediaTypeFromStreamDescriptorById call to typeHandler.GetMediaTypeByIndex failed. workingMediaType == null");
                        }
                        MFVideoFormatContainer tmpContainer = MediaTypeInfo.GetVideoFormatContainerFromMediaTypeObject(workingMediaType, UnderlyingDevice);
                        if (tmpContainer == null)
                        {
                            // we failed
                            throw new Exception("GetSupportedVideoFormatsFromSourceReaderInFormatContainers failed on call to GetVideoFormatContainerFromMediaTypeObject");
                        }
                        // now add it
                        formatList.Add(tmpContainer);
                        Marshal.ReleaseComObject(workingMediaType);
                        workingMediaType = null;
                    }
                    return formatList;
                }
                finally
                {
                    // close and release
                    if (mediaSource != null)
                    {
                        Marshal.ReleaseComObject(mediaSource);
                    }
                    if (sourcePresentationDescriptor != null)
                    {
                        Marshal.ReleaseComObject(sourcePresentationDescriptor);
                    }
                    if (videoStreamDescriptor != null)
                    {
                        Marshal.ReleaseComObject(videoStreamDescriptor);
                    }
                    if (typeHandler != null)
                    {
                        Marshal.ReleaseComObject(typeHandler);
                    }
                }
            }
            return new List<MFVideoFormatContainer>();
        }

        private void HandleMediaSessionAsyncCallBackErrors(object sender, string errMsg, Exception ex)
        {
            if (errMsg == null) errMsg = "unknown error";
            Console.WriteLine("HandleMediaSessionAsyncCallBackErrors Error" + errMsg);
            if (ex != null)
            {
                Console.WriteLine("HandleMediaSessionAsyncCallBackErrors Exception trace = " + ex.StackTrace);
            }
            // do everything to close all media devices
            CloseAllMediaDevices();
        }

        private void HandleMediaSessionAsyncCallBackEvent(object sender, IMFMediaEvent pEvent, MediaEventType mediaEventType)
        {
            Console.WriteLine("Media Event Type " + mediaEventType.ToString());
            switch (mediaEventType)
            {
                case MediaEventType.MESessionTopologyStatus:
                    // Raised by the Media Session when the status of a topology changes.
                    // Get the topology changed status code. This is an enum in the event
                    int i;
                    HResult hr = pEvent.GetUINT32(MFAttributesClsid.MF_EVENT_TOPOLOGY_STATUS, out i);
                    if (hr != HResult.S_OK)
                    {
                        throw new Exception("HandleMediaSessionAsyncCallBackEvent call to pEvent to get the status code failed. Err=" + hr.ToString());
                    }
                    // the one we are most interested in is i == MFTopoStatus.Ready
                    // which we get then the Topology is built and ready to run
                    HandleTopologyStatusChanged(mediaEventType, (MFTopoStatus)i);
                    break;

                case MediaEventType.MESessionStarted:
                    // Raised when the IMFMediaSession::Start method completes asynchronously.
                    //       PlayerState = EVRPlayerStateEnum.Started;
                    break;

                case MediaEventType.MESessionPaused:
                    // Raised when the IMFMediaSession::Pause method completes asynchronously.
                    //        PlayerState = EVRPlayerStateEnum.Paused;
                    break;

                case MediaEventType.MESessionStopped:
                    // Raised when the IMFMediaSession::Stop method completes asynchronously.
                    break;

                case MediaEventType.MESessionClosed:
                    // Raised when the IMFMediaSession::Close method completes asynchronously.
                    break;

                case MediaEventType.MESessionCapabilitiesChanged:
                    // Raised by the Media Session when the session capabilities change.
                    // You can use IMFMediaEvent::GetValue to figure out what they are
                    break;

                case MediaEventType.MESessionTopologySet:
                    // Raised after the IMFMediaSession::SetTopology method completes asynchronously.
                    // The Media Session raises this event after it resolves the topology into a full topology and queues the topology for playback.
                    break;

                case MediaEventType.MESessionNotifyPresentationTime:
                    // Raised by the Media Session when a new presentation starts.
                    // This event indicates when the presentation will start and the offset between the presentation time and the source time.
                    break;

                case MediaEventType.MEEndOfPresentation:
                    // Raised by a media source when a presentation ends. This event signals that all streams
                    // in the presentation are complete. The Media Session forwards this event to the application.
                    // we cannot sucessfully .Finalize_ on the SinkWriter
                    // if we call CloseAllMediaDevices directly from this thread
                    // so we use an asynchronous method
                    Task taskA = Task.Run(() => CloseAllMediaDevices());
                    break;

                case MediaEventType.MESessionRateChanged:
                    // Raised by the Media Session when the playback rate changes. This event is sent after the
                    // IMFRateControl::SetRate method completes asynchronously.
                    break;

                default:
                    Console.WriteLine("Unhandled Media Event Type " + mediaEventType.ToString());
                    break;
            }
        }

        private void HandleTopologyStatusChanged(MediaEventType mediaEventType, MFTopoStatus topoStatus)
        {
            Console.WriteLine("HandleTopologyStatusChanged event type: " + mediaEventType.ToString() + ", topoStatus=" + topoStatus.ToString());
            // we currently arent paying attention to these
            return;
        }

        private void MFBPressed(object sender, FrameReadyEventArgs e)
        {
            if (!Paused)
                OnMFB?.Invoke(this, e);
        }

        private void ProcessFrame(object sender, FrameReadyEventArgs e)
        {
            if (!Paused)
                FrameReady?.Invoke(this, e);
        }

        private void SetCurrentMediaType(IMFSourceReaderAsync reader, int IndexOfSelectSource, int StreamIndex)
        {
            List<IMFMediaType> list = new List<IMFMediaType>();
            HResult hr = reader.GetNativeMediaType(StreamIndex, list.Count, out IMFMediaType ppMediaType);
            while (hr != HResult.MF_E_NO_MORE_TYPES)
            {
                list.Add(ppMediaType);
                hr = reader.GetNativeMediaType(StreamIndex, list.Count, out ppMediaType);
            }
            reader.SetCurrentMediaType(StreamIndex, null, list[IndexOfSelectSource]);
        }

        public override void Load(string FriendlyName, int FrameRate, int Height, int Width, string Encoding)
        {
            HResult Return = MFExtern.MFStartup(0x00020070, MFStartup.Full);
            if (Return != 0)
            {
                Console.WriteLine("Constructor: call to MFExtern.MFStartup returned " + Return.ToString());
            }
            List<MFDevice> vcDevices = WMFUtils.GetDevicesByCategory(MFAttributesClsid.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE, CLSID.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_GUID);
            foreach (MFDevice device in vcDevices)
            {
                if (device.FriendlyName == FriendlyName)
                {
                    UnderlyingDevice = device;
                    break;
                }
            }
            IMFStreamIndex = GetRequestedStreamFormat(FrameRate, Width, Height, Encoding);
            IMFSnapIndex = GetRequestedSnapFormat(Width, Height, Encoding);
        }

        public override List<VideoType> ScanDevice(string friendlyName)
        {
            List<VideoType> types = new List<VideoType>();
            List<MFVideoFormatContainer> Formats = GetSupportedFormats(0, friendlyName);
            foreach (MFVideoFormatContainer x in Formats)
                types.Add(new VideoType(friendlyName, x.FrameRate, x.FrameSizeHeight, x.FrameSizeWidth, x.SubTypeAsString));
            return types;
        }

        public override List<string> Scan()
        {
            List<string> Devices = new List<string>();
            List<MFDevice> vcDevices = WMFUtils.GetDevicesByCategory(MFAttributesClsid.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE, CLSID.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_GUID);
            foreach (MFDevice device in vcDevices)
                Devices.Add(device.FriendlyName);
            return Devices;
        }
    }
}