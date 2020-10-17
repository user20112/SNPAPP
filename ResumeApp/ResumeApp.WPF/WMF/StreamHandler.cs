using MediaFoundation;
using MediaFoundation.Alt;
using MediaFoundation.Misc;
using MediaFoundation.ReadWrite;
using ResumeApp.Events;
using SkiaSharp;
using System;
using System.Runtime.InteropServices;

namespace ResumeApp.WPF.Classes.WMF
{
    /// <summary>
    /// A class to handle async SourceReader callbacks. This class is populated
    /// appropriately and then fed into the SourceReader. Once the first
    /// ReadSample is called on the SourceReader the code in this class is
    /// called and it continues requesting and processing new video frames
    /// indefinitely
    /// Note: The callback interface must be thread-safe, because OnReadSample
    /// and the other callback methods are called from worker threads.
    ///
    /// </summary>
    public class StreamHandler : COMBase, IMFSourceReaderCallback, IDisposable
    {
        private readonly EventHandler<FrameReadyEventArgs> OnMFB;
        private bool HandlingImage = false;
        private EventHandler<FrameReadyEventArgs> OnFrame;

        public StreamHandler(EventHandler<FrameReadyEventArgs> frameReceived, EventHandler<FrameReadyEventArgs> onMFBtemp)
        {
            OnFrame = frameReceived;
            OnMFB = onMFBtemp;
        }

        public IMFSourceReaderAsync StreamReader { get; set; }

        public void Dispose()
        {
            OnFrame = delegate { };
        }

        /// <summary>
        /// Inits this class for the first sample
        /// </summary>
        public void InitForFirstSample()
        {
        }

        public HResult OnEvent(int a, IMFMediaEvent b)
        {
            return HResult.S_OK;
        }

        public HResult OnFlush(int a)
        {
            return HResult.S_OK;
        }

        /// <summary>
        /// This gets called when a Called IMFSourceReader.ReadSample method completes
        /// (assuming the SourceReader has been given this class during setup with
        /// an attribute of MFAttributesClsid.MF_SOURCE_READER_ASYNC_CALLBACK).
        /// The first ReadSample triggers it after that it continues by itself
        /// </summary>
        /// <param name="hrStatus">The status code. If an error occurred while processing the next sample, this parameter contains the error code.</param>
        /// <param name="streamIndex">The zero-based index of the stream that delivered the sample.</param>
        /// <param name="streamFlags">A bitwise OR of zero or more flags from the MF_SOURCE_READER_FLAG enumeration.</param>
        /// <param name="sampleTimeStamp">The time stamp of the sample, or the time of the stream event indicated in streamFlags. The time is given in 100-nanosecond units. </param>
        /// <param name="mediaSample">A pointer to the IMFSample interface of a media sample. This parameter might be NULL.</param>
        /// <returns>Returns an HRESULT value. Reputedly, the source reader ignores the return value.</returns>
        public HResult OnReadSample(HResult hrStatus, int streamIndex, MF_SOURCE_READER_FLAG streamFlags, long sampleTimeStamp, IMFSample mediaSample)
        {
            HResult hr = HResult.S_OK;
            switch (streamIndex)
            {
                case 0:
                    Console.WriteLine(streamFlags);
                    break;

                case 1:
                    break;
            }
            try
            {
                lock (this)
                {
                    // have we got an error?
                    if (Failed(hrStatus))
                    {
                    }
                    else
                    {
                        // have we got a sample? It seems this can be null on the first sample
                        // in after the ReadSample that triggered this. So we just ignore it
                        // and request the next to get things rolling
                        if (mediaSample != null)
                        {
                            if (streamIndex == 1)
                            {
                            }
                            try
                            {
                                if (!HandlingImage)
                                {
                                    HandlingImage = true;
                                    mediaSample.GetBufferByIndex(0, out IMFMediaBuffer Buffer);
                                    Buffer.Lock(out IntPtr bufPointer, out int MaxLength, out int CurrLength);
                                    byte[] managedArray = new byte[MaxLength * 480];
                                    Marshal.Copy(bufPointer, managedArray, 0, MaxLength);
                                    Buffer.Unlock();
                                    SKImage temp = SKImage.FromEncodedData(SKImage.FromEncodedData(managedArray).Encode());
                                    switch (streamIndex)
                                    {
                                        case 0:
                                            OnFrame(this, new FrameReadyEventArgs { FrameBuffer = null, Image = temp });
                                            break;

                                        case 1:
                                            OnMFB(this, new FrameReadyEventArgs { FrameBuffer = null, Image = temp });
                                            break;
                                    }
                                    HandlingImage = false;
                                }
                            }
                            catch
                            {
                                HandlingImage = false;
                            }
                        }
                        else
                        {
                        }
                    }
                    switch (streamIndex)
                    {
                        case 0:
                            //StreamReader.Flush(1);
                            // Read another sample.
                            hr = StreamReader.ReadSample(
                                  0,
                                   MediaFoundation.ReadWrite.MF_SOURCE_READER_CONTROL_FLAG.None,
                                   IntPtr.Zero,
                                   IntPtr.Zero,
                                   IntPtr.Zero,
                                   IntPtr.Zero
                                   );
                            // Read another sample.
                            hr = StreamReader.ReadSample(
                                   1,
                                   MediaFoundation.ReadWrite.MF_SOURCE_READER_CONTROL_FLAG.Drain,
                                   IntPtr.Zero,
                                   IntPtr.Zero,
                                   IntPtr.Zero,
                                   IntPtr.Zero
                                   );
                            break;
                    }
                }
            }
            catch
            {
            }
            finally
            {
                SafeRelease(mediaSample);
            }
            return hr;
        }
    }
}