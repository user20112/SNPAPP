using MediaFoundation;
using MediaFoundation.Misc;
using System;
using System.Runtime.InteropServices;

namespace ResumeApp.WPF.Classes.WMF
{
    /// <summary>
    /// A class to handle IMFAsyncCallback callbacks. This class is given an
    /// IMFMediaSession object and once it has that it handles messages from the
    /// session and performs the apropriate event notifications
    ///
    /// Note: The callback interface must be thread-safe, because OnReadSample
    /// and the other callback methods are called from worker threads.
    ///
    /// </summary>
    public class AsyncCallbackHandler : COMBase, IMFAsyncCallback
    {
        public MediaSessionAsyncCallBackError_Delegate MediaSessionAsyncCallBackError = null;
        public MediaSessionAsyncCallBackEvent_Delegate MediaSessionAsyncCallBackEvent = null;

        // our MediaSession object.
        private IMFMediaSession mediaSession = null;

        // our error reporting delegate + event
        public delegate void MediaSessionAsyncCallBackError_Delegate(object sender, string errMsg, Exception ex);

        // our media session event reporting delegate + event
        public delegate void MediaSessionAsyncCallBackEvent_Delegate(object sender, IMFMediaEvent eventObj, MediaEventType mediaEventType);

        /// <summary>
        /// gets/sets the mediaSession. Will get/set null
        /// </summary>
        public IMFMediaSession MediaSession
        {
            get
            {
                return mediaSession;
            }
            set
            {
                mediaSession = value;
            }
        }

        /// <summary>
        /// Part of the IMFAsyncCallback interface. Provides configuration information
        /// to the dispatching thread for a callback.
        /// </summary>
        /// <param name="pdwFlags">Receives a flag indicating the behavior of the callback object's IMFAsyncCallback::Invoke method.</param>
        /// <param name="pdwQueue">Receives the identifier of the work queue on which the callback is dispatched.</param>
        HResult IMFAsyncCallback.GetParameters(out MFASync pdwFlags, out MFAsyncCallbackQueue pdwQueue)
        {
            // We are saying the callback does very minimal processing. It takes less than 1 millisecond to complete.
            // This callback must be invoked from one of the following work queues:
            //   MFASYNC_CALLBACK_QUEUE_IO
            //   MFASYNC_CALLBACK_QUEUE_TIMER
            pdwFlags = MFASync.FastIOProcessingCallback;
            // The docs say that in most cases, applications should use MFASYNC_CALLBACK_QUEUE_MULTITHREADED.
            // We use the standard here which is used for synchronous operations. Using the standard work queue may run the risk of deadlocking.
            pdwQueue = MFAsyncCallbackQueue.Standard;
            // return our configuration
            return HResult.S_OK;
        }

        /// <summary>
        /// Initializes this class
        /// </summary>
        public void Initialize()
        {
        }

        /// <summary>
        /// Part of the IMFAsyncCallback interface. This is called when an
        /// asynchronous operation is completed.
        /// </summary>
        /// <param name="pResult">Pointer to the IMFAsyncResult interface. </param>
        /// <returns>S_OK for success, others for fail</returns>
        HResult IMFAsyncCallback.Invoke(IMFAsyncResult pResult)
        {
            HResult hr;
            IMFMediaEvent eventObj = null;
            MediaEventType meType = MediaEventType.MEUnknown;  // Event type
            HResult hrStatus = 0;           // Event status
            lock (this)
            {
                try
                {
                    if (MediaSession == null) return HResult.S_OK;
                    // Complete the asynchronous request this is tied to the previous BeginGetEvent call
                    // and MUST be done. The output here is a pointer to the IMFMediaEvent interface describing
                    // this event. Note we MUST release this interface
                    hr = MediaSession.EndGetEvent(pResult, out eventObj);
                    if (hr != HResult.S_OK)
                    {
                        throw new Exception("IMFAsyncCallback.Invoke call to MediaSession.EndGetEvent failed. Err=" + hr.ToString());
                    }
                    if (eventObj == null)
                    {
                        throw new Exception("IMFAsyncCallback.Invoke call to MediaSession.EndGetEvent failed. eventObj == null");
                    }
                    // Get the event type. The event type indicates what happened to trigger the event.
                    // It also defines the meaning of the event value.
                    hr = eventObj.GetType(out meType);
                    if (hr != HResult.S_OK)
                    {
                        throw new Exception("IMFAsyncCallback.Invoke call to IMFMediaEvent.GetType failed. Err=" + hr.ToString());
                    }
                    // Get the event status. If the operation that generated the event was successful,
                    // the value is a success code. A failure code means that an error condition triggered the event.
                    hr = eventObj.GetStatus(out hrStatus);
                    if (hr != HResult.S_OK)
                    {
                        throw new Exception("IMFAsyncCallback.Invoke call to IMFMediaEvent.GetStatus failed. Err=" + hr.ToString());
                    }
                    // Check if we are being told that the the async event succeeded.
                    if (hrStatus != HResult.S_OK)
                    {
                        // The async operation failed. Notify the application
                        MediaSessionAsyncCallBackError?.Invoke(this, "Error Code =" + hrStatus.ToString(), null);
                    }
                    else
                    {
                        // we are being told the operation succeeded and therefore the event contents are meaningful.
                        // Switch on the event type.
                        switch (meType)
                        {
                            // we let the app handle all of these. There is not really much we can do here
                            default:
                                MediaSessionAsyncCallBackEvent(this, eventObj, meType);
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // The async operation failed. Notify the application
                    MediaSessionAsyncCallBackError?.Invoke(this, ex.Message, ex);
                }
                finally
                {
                    // Request another event if we are still operational.
                    if (((meType == MediaEventType.MESessionClosed) || (meType == MediaEventType.MEEndOfPresentation)) == false)
                    {
                        // Begins an asynchronous request for the next event in the queue
                        hr = MediaSession.BeginGetEvent(this, null);
                        if (hr != HResult.S_OK)
                        {
                            throw new Exception("IMFAsyncCallback.Invoke call to MediaSession.BeginGetEvent failed. Err=" + hr.ToString());
                        }
                    }
                    // release the event we just processed
                    if (eventObj != null)
                    {
                        Marshal.ReleaseComObject(eventObj);
                    }
                }
            } // bottom of lock(this)
            return HResult.S_OK;
        }

        /// <summary>
        /// Shuts down this class
        /// </summary>
        public void ShutDown()
        {
            lock (this)
            {
                MediaSessionAsyncCallBackEvent = null;
                MediaSessionAsyncCallBackError = null;
            }
        }
    }
}