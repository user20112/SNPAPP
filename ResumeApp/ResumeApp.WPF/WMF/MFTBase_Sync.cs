﻿using MediaFoundation;
using MediaFoundation.Misc;
using MediaFoundation.Transform;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ResumeApp.WPF.Classes.WMF
{
    abstract public class MFTBase_Sync : COMObjBase, IMFTransform
    {
        /// Synchronization object used by public entry points
        /// to prevent multiple methods from being invoked at
        /// once.  Some work (for example parameter validation)
        /// is done before invoking this lock.
        private object m_TransformLockObject;

        /// Input type set by SetInputType.
        /// Can be null if not set yet or if reset by SetInputType(null).
        private IMFMediaType m_pInputType;

        /// Output type set by SetOutputType.
        /// Can be null if not set yet or if reset by SetOutputType(null).
        private IMFMediaType m_pOutputType;

        /// The most recent sample received by ProcessInput, or null if no sample is pending.
        private IMFSample m_pSample;

        /// this attribute collection is not strictly required in
        /// sync mode transforms as no part of the configuration
        /// and I/O type negotiation process uses these. However,
        /// the client can access the attributes in this object at
        /// runtime via the TopologyNode of the transform so we
        /// can use these attributes to send and receive certain
        /// types of configuration information.
        private readonly IMFAttributes m_TransformAttributeCollection;

        /// <summary>
        /// Constructor
        /// </summary>
        protected MFTBase_Sync()
        {
            //DebugMessage("MFTBase_Sync MFTImplementation");
            m_TransformLockObject = new object();
            m_pInputType = null;
            m_pOutputType = null;
            m_pSample = null;
            // Build the IMFAttributes we use. We give it three by default.
            MFExtern.MFCreateAttributes(out m_TransformAttributeCollection, 3);
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~MFTBase_Sync()
        {
            //DebugMessage("MFTBase_Sync ~MFTImplementation");
            // release any COM objects we have
            if (m_TransformLockObject != null)
            {
                SafeRelease(m_pInputType);
                SafeRelease(m_pOutputType);
                SafeRelease(m_pSample);
                SafeRelease(m_TransformAttributeCollection);
                m_TransformLockObject = null;
            }
        }

        // ########################################################################
        // ##### Overrides, child classes must implement these
        // ########################################################################
        /// <summary>
        /// Report whether a proposed input type is accepted by the MFT.
        /// </summary>
        /// <param name="pmt">The type to check.  Should never be null.</param>
        /// <returns>S_Ok if the type is valid or MF_E_INVALIDTYPE.</returns>
        abstract protected HResult OnCheckInputType(IMFMediaType pmt);

        /// <summary>
        /// Return settings to describe input stream
        /// (see IMFTransform::GetInputStreamInfo).
        /// </summary>
        /// <param name="pStreamInfo">The struct where the parameters get set.</param>
        abstract protected void OnGetInputStreamInfo(ref MFTInputStreamInfo pStreamInfo);

        /// <summary>
        /// Return settings to describe output stream
        /// (see IMFTransform::GetOutputStreamInfo).
        /// </summary>
        /// <param name="pStreamInfo">The struct where the parameters get set.</param>
        abstract protected void OnGetOutputStreamInfo(ref MFTOutputStreamInfo pStreamInfo);

        /// <summary>
        /// The routine that usually performs the transform.
        ///
        /// The input sample is in InputSample.  Process it into the pOutputSamples struct.
        /// Depending on what you set in On*StreamInfo, you can either perform
        /// in-place processing by modifying the input sample (which still must
        /// set inout the struct), or create a new IMFSample and FULLY populate
        /// it from the input.  If the input sample has been fully processed,
        /// set InputSample to null.
        /// </summary>
        /// <param name="pOutputSamples">The structure to populate with output values.</param>
        /// <returns>S_Ok unless error.</returns>
        abstract protected HResult OnProcessOutput(ref MFTOutputDataBuffer pOutputSamples);

        /// <summary>
        /// Report whether a proposed output type is accepted by the MFT. The
        /// default behavior is to assume that the input type
        /// must be set before the output type, and that the proposed output
        /// type must exactly equal the value returned from the virtual
        /// CreateOutputFromInput.  Override as necessary
        /// </summary>
        /// <param name="pmt">The type to check.  Should never be null (which are always valid).</param>
        /// <returns>S_Ok if the type is valid or MF_E_INVALIDTYPE.</returns>
        virtual protected HResult OnCheckOutputType(IMFMediaType pmt)
        {
            HResult hr = HResult.S_OK;
            // If the input type is set, see if they match.
            if (m_pInputType != null)
            {
                IMFMediaType pCheck = CreateOutputFromInput();
                try
                {
                    hr = WMFUtils.IsMediaTypeIdentical(pmt, pCheck);
                }
                finally
                {
                    SafeRelease(pCheck);
                }
            }
            else
            {
                // Input type is not set.
                hr = HResult.MF_E_TRANSFORM_TYPE_NOT_SET;
            }
            return hr;
        }

        /// <summary>
        /// Override to get notified when the Input Type is actually being set.
        /// </summary>
        /// <remarks>The new type is in InputType, and can be null.</remarks>
        virtual protected void OnSetInputType()
        {
        }

        /// <summary>
        /// Override to get notified when the Output Type is actually being set.
        /// </summary>
        /// <remarks>The new type is in OutputType, and can be null.</remarks>
        virtual protected void OnSetOutputType()
        {
        }

        /// <summary>
        /// Override to allow the client to retrieve the MFT's list of supported
        /// Input Types.
        ///
        /// This method is virtual since it is (sort of) optional.
        /// For example, if a client *knows* what types the MFT supports, it can
        /// just set it.  Not all clients support MFTs that won't enum types.
        /// </summary>
        /// <param name="dwTypeIndex">The (zero-based) index of the type.</param>
        /// <param name="pInputType">The input type supported by the MFT.</param>
        /// <returns>S_Ok unless error.</returns>
        virtual protected HResult OnEnumInputTypes(int dwTypeIndex, out IMFMediaType pInputType)
        {
            pInputType = null;
            return HResult.E_NOTIMPL;
        }

        /// <summary>
        /// Override to allow the client to retrieve the MFT's list of supported Output Types.
        ///
        /// By default, assume the input type must be set first, and
        /// that the output type is the single entry returned from the virtual
        /// CreateOutputFromInput.  Override as needed.
        /// </summary>
        /// <param name="dwTypeIndex">The (zero-based) index of the type.</param>
        /// <param name="pOutputType">The output type supported by the MFT.</param>
        /// <returns>S_Ok or MFError.MF_E_NO_MORE_TYPES.</returns>
        virtual protected HResult OnEnumOutputTypes(int dwTypeIndex, out IMFMediaType pOutputType)
        {
            HResult hr = HResult.S_OK;
            // If the input type is specified, the output type must be the same.
            if (m_pInputType != null)
            {
                // If the input type is specified, there can be only one output type.
                if (dwTypeIndex == 0)
                {
                    pOutputType = CreateOutputFromInput();
                }
                else
                {
                    pOutputType = null;
                    hr = HResult.MF_E_NO_MORE_TYPES;
                }
            }
            else
            {
                pOutputType = null;
                hr = HResult.MF_E_TRANSFORM_TYPE_NOT_SET;
            }
            return hr;
        }

        /// <summary>
        /// A new input sample has been received.
        /// </summary>
        /// <returns>S_Ok unless error.</returns>
        /// <remarks>The sample is in InputSample.  Typically nothing is done
        /// here.  The processing is done in OnProcessOutput, when we have
        /// the output buffer.</remarks>
        virtual protected HResult OnProcessInput()
        {
            return HResult.S_OK;
        }

        /// <summary>
        /// Called at end of stream (and start of new stream).
        /// </summary>
        virtual protected void OnReset()
        {
        }

        /// <summary>
        /// Create a single output type from an input type.
        ///
        /// In many cases, there is only one possible output type, and it is
        /// either identical to, or a direct consequence of the input type.
        /// Provided with that single output type, OnCheckOutputType and
        /// OnEnumOutputTypes can be written generically, so they don't have
        /// to be implemented by the derived class.  At worst, this one method
        /// may need to be overridden.
        /// </summary>
        /// <returns>By default, a clone of the input type.  Can be overridden.</returns>
        virtual protected IMFMediaType CreateOutputFromInput()
        {
            return WMFUtils.CloneMediaType(m_pInputType);
        }

        // ########################################################################
        // ##### IMFTransform methods
        // ########################################################################
        /// <summary>
        /// Gets the minimum and maximum number of input and output streams for
        /// this transform
        /// </summary>
        /// <param name="pdwInputMaximum">Receives the maximum number of input streams. If there is no maximum, receives the value MFT_STREAMS_UNLIMITED</param>
        /// <param name="pdwInputMinimum">Receives the minimum number of input streams</param>
        /// <param name="pdwOutputMaximum">Receives the maximum number of output streams. If there is no maximum, receives the value MFT_STREAMS_UNLIMITED. </param>
        /// <param name="pdwOutputMinimum">Receives the minimum number of output streams</param>
        /// <returns>S_OK or other for fail</returns>
        public HResult GetStreamLimits(MFInt pdwInputMinimum, MFInt pdwInputMaximum, MFInt pdwOutputMinimum, MFInt pdwOutputMaximum)
        {
            HResult hr = HResult.S_OK;
            try
            {
                //DebugMessage("GetStreamLimits GetStreamLimits");
                // This template requires a fixed number of input and output
                // streams (1 for each).
                lock (m_TransformLockObject)
                {
                    // Fixed stream limits.
                    if (pdwInputMinimum != null)
                    {
                        pdwInputMinimum.Assign(1);
                    }
                    if (pdwInputMaximum != null)
                    {
                        pdwInputMaximum.Assign(1);
                    }
                    if (pdwOutputMinimum != null)
                    {
                        pdwOutputMinimum.Assign(1);
                    }
                    if (pdwOutputMaximum != null)
                    {
                        pdwOutputMaximum.Assign(1);
                    }
                }
            }
            catch (Exception e)
            {
                hr = (HResult)Marshal.GetHRForException(e);
            }
            return CheckReturn(hr);
        }

        /// <summary>
        /// Gets the current number of input and output streams on this transform.
        /// The number of streams includes unselected streams — that is,
        /// streams with no media type or a NULL media type.
        ///
        /// </summary>
        /// <param name="pcInputStreams">Receives the number of input streams</param>
        /// <param name="pcOutputStreams">Receives the number of output streams</param>
        /// <returns>S_OK or other for fail</returns>
        public HResult GetStreamCount(MFInt pcInputStreams, MFInt pcOutputStreams)
        {
            HResult hr = HResult.S_OK;
            try
            {
                lock (m_TransformLockObject)
                {
                    // This template requires a fixed number of input and output
                    // streams (1 for each).
                    if (pcInputStreams != null)
                    {
                        pcInputStreams.Assign(1);
                    }
                    if (pcOutputStreams != null)
                    {
                        pcOutputStreams.Assign(1);
                    }
                }
            }
            catch (Exception e)
            {
                hr = (HResult)Marshal.GetHRForException(e);
            }
            return hr;
        }

        /// <summary>
        /// Gets the stream identifiers for the input and output streams on this
        /// transform
        ///
        /// </summary>
        /// <param name="dwInputIDArraySize">Number of elements in the pdwInputIDs array</param>
        /// <param name="dwOutputIDArraySize">Number of elements in the pdwOutputIDs array</param>
        /// <param name="pdwInputIDs">Pointer to an array allocated by the caller. The method fills
        /// the array with the input stream identifiers. The array size must be at least equal to the number of input streams.
        /// To get the number of input streams, call IMFTransform::GetStreamCount</param>
        /// <param name="pdwOutputIDs">Pointer to an array allocated by the caller. The method fills
        /// the array with the output stream identifiers. The array size must be at least equal to
        /// the number of output streams. To get the number of output streams, call GetStreamCount. </param>
        /// <returns>S_OK or other for fail</returns>
        public HResult GetStreamIDs(int dwInputIDArraySize, int[] pdwInputIDs, int dwOutputIDArraySize, int[] pdwOutputIDs)
        {
            HResult hr = HResult.S_OK;
            try
            {
                lock (m_TransformLockObject)
                {
                    // MF.Net sample notes
                    // Since our stream counts are fixed, we don't need
                    // to implement this method.  As a result, our input
                    // and output streams are always #0.
                    hr = HResult.E_NOTIMPL;
                }
            }
            catch (Exception e)
            {
                hr = (HResult)Marshal.GetHRForException(e);
            }
            return hr; // CheckReturn(hr);
        }

        /// <summary>
        /// Gets the buffer requirements and other information for an input stream on
        /// this transform
        ///
        /// </summary>
        /// <param name="dwInputStreamID">Input stream identifier. To get the list of stream
        /// identifiers, call IMFTransform::GetStreamIDs</param>
        /// <param name="pStreamInfo">Pointer to an MFT_INPUT_STREAM_INFO structure.
        /// The method fills the structure with information about the input stream. </param>
        /// <returns>S_OK or other for fail</returns>
        public HResult GetInputStreamInfo(int dwInputStreamID, out MFTInputStreamInfo pStreamInfo)
        {
            // Overwrites everything with zeros.
            pStreamInfo = new MFTInputStreamInfo();
            HResult hr = HResult.S_OK;
            try
            {
                //DebugMessage("GetInputStreamInfo");
                CheckValidStream(dwInputStreamID);
                lock (m_TransformLockObject)
                {
                    // The implementation can override the defaults,
                    // and must set cbSize
                    OnGetInputStreamInfo(ref pStreamInfo);
                }
            }
            catch (Exception e)
            {
                hr = (HResult)Marshal.GetHRForException(e);
            }
            return CheckReturn(hr);
        }

        /// <summary>
        /// Gets the buffer requirements and other information for an output stream on
        /// this transform.
        ///
        /// </summary>
        /// <param name="dwOutputStreamID">Output stream identifier. </param>
        /// <param name="pStreamInfo">Pointer to an MFT_OUTPUT_STREAM_INFO structure.
        /// The method fills the structure with information about the output stream. </param>
        /// <returns>S_OK or other for fail</returns>
        public HResult GetOutputStreamInfo(int dwOutputStreamID, out MFTOutputStreamInfo pStreamInfo)
        {
            // Overwrites everything with zeros.
            pStreamInfo = new MFTOutputStreamInfo();
            HResult hr = HResult.S_OK;
            try
            {
                //DebugMessage("GetOutputStreamInfo");
                CheckValidStream(dwOutputStreamID);
                lock (m_TransformLockObject)
                {
                    // The implementation can override the defaults,
                    // and must set cbSize.
                    OnGetOutputStreamInfo(ref pStreamInfo);
                }
            }
            catch (Exception e)
            {
                hr = (HResult)Marshal.GetHRForException(e);
            }
            return CheckReturn(hr);
        }

        /// <summary>
        /// Gets the global attribute store for this Transform
        ///
        /// </summary>
        /// <param name="pAttributes">Receives a pointer to the IMFAttributes interface.
        /// The caller must release the interface.</param>
        /// <returns>S_OK or other for fail</returns>
        public HResult GetAttributes(out IMFAttributes pAttributes)
        {
            pAttributes = null;
            HResult hr = HResult.S_OK;
            try
            {
                // DebugMessage("GetAttributes"); not interesting
                // Do not check CheckUnlocked (per spec)
                lock (m_TransformLockObject)
                {
                    // Using GetUniqueRCW means the caller can do
                    // ReleaseComObject without trashing our copy.  We *don't*
                    // want to return a clone because we *do* want them to be
                    // able to change our attributes.
                    pAttributes = WMFUtils.GetUniqueRCW(m_TransformAttributeCollection) as IMFAttributes;
                }
            }
            catch (Exception e)
            {
                hr = (HResult)Marshal.GetHRForException(e);
            }
            return hr;
        }

        /// <summary>
        /// Gets the attribute store for an input stream on this Transform.
        ///
        /// </summary>
        /// <param name="dwInputStreamID">Input stream identifier.</param>
        /// <param name="pAttributes">Receives a pointer to the IMFAttributes interface.
        /// The caller must release the interface.</param>
        /// <returns>S_OK or other for fail</returns>
        public HResult GetInputStreamAttributes(int dwInputStreamID, out IMFAttributes pAttributes)
        {
            pAttributes = null;
            HResult hr = HResult.S_OK;
            try
            {
                // Trace("GetInputStreamAttributes"); Not interesting
                CheckValidStream(dwInputStreamID);
                lock (m_TransformLockObject)
                {
                    hr = HResult.E_NOTIMPL;
                }
            }
            catch (Exception e)
            {
                hr = (HResult)Marshal.GetHRForException(e);
            }
            return hr; // CheckReturn(hr);
        }

        /// <summary>
        /// Gets the attribute store for an output stream on this Transform.
        ///
        /// </summary>
        /// <param name="dwOutputStreamID">Output stream identifier.</param>
        /// <param name="pAttributes">Receives a pointer to the IMFAttributes interface. T
        /// he caller must release the interface.</param>
        /// <returns>S_OK or other for fail</returns>
        public HResult GetOutputStreamAttributes(int dwOutputStreamID, out IMFAttributes pAttributes)
        {
            pAttributes = null;
            HResult hr = HResult.S_OK;
            try
            {
                CheckValidStream(dwOutputStreamID);
                lock (m_TransformLockObject)
                {
                    hr = HResult.E_NOTIMPL;
                }
            }
            catch (Exception e)
            {
                hr = (HResult)Marshal.GetHRForException(e);
            }
            return hr; // CheckReturn(hr);
        }

        /// <summary>
        /// Removes an input stream from this Transform .
        ///
        /// </summary>
        /// <param name="dwInputStreamID">Input stream identifier.</param>
        /// <returns>S_OK or other for fail</returns>
        public HResult DeleteInputStream(int dwStreamID)
        {
            HResult hr = HResult.S_OK;
            try
            {
                //DebugMessage("DeleteInputStream");
                CheckValidStream(dwStreamID);
                lock (m_TransformLockObject)
                {
                    // Removing streams not supported.
                    hr = HResult.E_NOTIMPL;
                }
            }
            catch (Exception e)
            {
                hr = (HResult)Marshal.GetHRForException(e);
            }
            return CheckReturn(hr);
        }

        /// <summary>
        /// Adds one or more new input streams to this Transform .
        ///
        /// </summary>
        /// <param name="adwStreamIDs">Array of stream identifiers. The new stream
        /// identifiers must not match any existing input streams</param>
        /// <param name="cStreams">Number of streams to add</param>
        /// <returns>S_OK or other for fail</returns>
        public HResult AddInputStreams(int cStreams, int[] adwStreamIDs)
        {
            HResult hr = HResult.S_OK;
            try
            {
                //DebugMessage("AddInputStreams");
                lock (m_TransformLockObject)
                {
                    // Adding streams not supported.
                    hr = HResult.E_NOTIMPL;
                }
            }
            catch (Exception e)
            {
                hr = (HResult)Marshal.GetHRForException(e);
            }
            return CheckReturn(hr);
        }

        /// <summary>
        /// Gets an available media type for an input stream on this Transform.
        ///
        /// </summary>
        /// <param name="dwInputStreamID">Input stream identifier. To get the list
        /// of stream identifiers, call IMFTransform::GetStreamIDs.</param>
        /// <param name="dwTypeIndex">Index of the media type to retrieve. Media
        /// types are indexed from zero and returned in approximate order of preference.</param>
        /// <param name="ppType">Receives a pointer to the IMFMediaType interface</param>
        /// <returns>S_OK or other for fail</returns>
        public HResult GetInputAvailableType(int dwInputStreamID, int dwTypeIndex, out IMFMediaType ppType)
        {
            ppType = null;
            HResult hr = HResult.S_OK;
            try
            {
                //DebugMessage(string.Format("GetInputAvailableType (stream = {0}, type index = {1})", dwInputStreamID, dwTypeIndex));
                CheckValidStream(dwInputStreamID);
                lock (m_TransformLockObject)
                {
                    // Get the input media type from the derived class.
                    // No need to pass dwInputStreamID, since it must
                    // always be zero.
                    hr = OnEnumInputTypes(dwTypeIndex, out ppType);
                }
            }
            catch (Exception e)
            {
                hr = (HResult)Marshal.GetHRForException(e);
            }
            return CheckReturn(hr);
        }

        /// <summary>
        /// Gets an available media type for an output stream on this Transform.
        ///
        /// </summary>
        /// <param name="dwOutputStreamID">Output stream identifier. To get the
        /// list of stream identifiers, call IMFTransform::GetStreamIDs.</param>
        /// <param name="dwTypeOutdex">Outdex of the media type to retrieve.
        /// Media types are outdexed from zero and returned out approximate order of preference.</param>
        /// <param name="ppType">Receives a pooutter to the IMFMediaType interface</param>
        /// <returns>S_OK or other for fail</returns>
        public HResult GetOutputAvailableType(int dwOutputStreamID, int dwTypeIndex, out IMFMediaType ppType)
        {
            ppType = null;
            HResult hr = HResult.S_OK;
            try
            {
                //DebugMessage(string.Format("GetOutputAvailableType (stream = {0}, type index = {1})", dwOutputStreamID, dwTypeIndex));
                CheckValidStream(dwOutputStreamID);
                lock (m_TransformLockObject)
                {
                    // Get the output media type from the derived class.
                    // No need to pass dwOutputStreamID, since it must
                    // always be zero.
                    hr = OnEnumOutputTypes(dwTypeIndex, out ppType);
                }
            }
            catch (Exception e)
            {
                hr = (HResult)Marshal.GetHRForException(e);
            }
            return CheckReturn(hr);
        }

        /// <summary>
        /// Sets, tests, or clears the media type for an input stream on this Transform.
        ///
        /// </summary>
        /// <param name="dwFlags">Zero or more flags from the _MFT_SET_TYPE_FLAGS enumeration.</param>
        /// <param name="dwInputStreamID">Input stream identifier. To get the list of
        /// stream identifiers, call IMFTransform::GetStreamIDs.</param>
        /// <param name="pType">Pointer to the IMFMediaType interface, or NULL. </param>
        /// <returns>S_OK or other for fail</returns>
        public HResult SetInputType(int dwInputStreamID, IMFMediaType pType, MFTSetTypeFlags dwFlags)
        {
            HResult hr = HResult.S_OK;
            try
            {
                //DebugMessage("SetInputType");
                CheckValidStream(dwInputStreamID);
                lock (m_TransformLockObject)
                {
                    // If we have an input sample, the client cannot change the type now.
                    if (!HasPendingOutput())
                    {
                        // Allow input type to be cleared.
                        if (pType != null)
                        {
                            // Validate non-null types.
                            hr = OnCheckInputType(pType);
                        }
                        if (Succeeded(hr))
                        {
                            // Does the caller want to set the type?  Or just test it?
                            bool bReallySet = ((dwFlags & MFTSetTypeFlags.TestOnly) == 0);
                            if (bReallySet)
                            {
                                // Make a copy of the IMFMediaType.
                                InputType = WMFUtils.CloneMediaType(pType);
                                OnSetInputType();
                            }
                        }
                    }
                    else
                    {
                        // Can't change type while samples are pending
                        hr = HResult.MF_E_TRANSFORM_CANNOT_CHANGE_MEDIATYPE_WHILE_PROCESSING;
                    }
                }
            }
            catch (Exception e)
            {
                hr = (HResult)Marshal.GetHRForException(e);
            }
            //finally
            {
                // MF.Net sample notes
                // While we *should* do this, we can't.  If the caller is c#, we
                // would destroy their copy.  Instead we have to leave this for
                // the GC.
                // SafeRelease(pType);
            }
            return CheckReturn(hr);
        }

        /// <summary>
        /// Sets, tests, or clears the media type for an output stream on this Transform.
        ///
        /// </summary>
        /// <param name="dwFlags">Zero or more flags from the _MFT_SET_TYPE_FLAGS enumeration.</param>
        /// <param name="dwOutputStreamID">Output stream identifier. To get the list of stream identifiers, call IMFTransform::GetStreamIDs.</param>
        /// <param name="pType">Pooutter to the IMFMediaType interface, or NULL. </param>
        /// <returns>S_OK or other for fail</returns>
        public HResult SetOutputType(int dwOutputStreamID, IMFMediaType pType, MFTSetTypeFlags dwFlags)
        {
            HResult hr = HResult.S_OK;
            try
            {
                //DebugMessage("SetOutputType");
                CheckValidStream(dwOutputStreamID);
                lock (m_TransformLockObject)
                {
                    // If we have an input sample, the client cannot change the type now.
                    if (!HasPendingOutput())
                    {
                        if (pType != null)
                        {
                            // Validate the type.
                            hr = OnCheckOutputType(pType);
                        }
                        if (Succeeded(hr))
                        {
                            // Does the caller want us to set the type, or just test it?
                            bool bReallySet = ((dwFlags & MFTSetTypeFlags.TestOnly) == 0);
                            // Set the type, unless the caller was just testing.
                            if (bReallySet)
                            {
                                // Make our own copy of the type.
                                OutputType = WMFUtils.CloneMediaType(pType);
                                OnSetOutputType();
                            }
                        }
                    }
                    else
                    {
                        // Cannot change type while samples are pending
                        hr = HResult.MF_E_TRANSFORM_CANNOT_CHANGE_MEDIATYPE_WHILE_PROCESSING;
                    }
                }
            }
            catch (Exception e)
            {
                hr = (HResult)Marshal.GetHRForException(e);
            }
            //finally
            {
                // While we *should* do this, we can't.  If the caller is c#, we
                // would destroy their copy.  Instead we have to leave this for
                // the GC.
                // SafeRelease(pType);
            }
            return CheckReturn(hr);
        }

        /// <summary>
        /// Gets the current media type for an input stream on this Transform.
        ///
        /// </summary>
        /// <param name="dwInputStreamID">Input stream identifier. To get the
        /// list of stream identifiers, call IMFTransform::GetStreamIDs. </param>
        /// <param name="ppType">Receives a pointer to the IMFMediaType
        /// interface. The caller must release the interface.</param>
        /// <returns>S_OK or other for fail</returns>
        public HResult GetInputCurrentType(int dwInputStreamID, out IMFMediaType ppType)
        {
            ppType = null;
            HResult hr = HResult.S_OK;
            try
            {
                //DebugMessage("GetInputCurrentType");
                CheckValidStream(dwInputStreamID);
                lock (m_TransformLockObject)
                {
                    if (m_pInputType != null)
                    {
                        ppType = WMFUtils.CloneMediaType(m_pInputType);
                    }
                    else
                    {
                        // Type is not set
                        hr = HResult.MF_E_TRANSFORM_TYPE_NOT_SET;
                    }
                }
            }
            catch (Exception e)
            {
                hr = (HResult)Marshal.GetHRForException(e);
            }
            return CheckReturn(hr);
        }

        /// <summary>
        /// Gets the current media type for an output stream on this Transform.
        ///
        /// </summary>
        /// <param name="dwOutputStreamID">Output stream identifier. To get the
        /// list of stream identifiers, call IMFTransform::GetStreamIDs. </param>
        /// <param name="ppType">Receives a pooutter to the IMFMediaType
        /// interface. The caller must release the interface.</param>
        /// <returns>S_OK or other for fail</returns>
        public HResult GetOutputCurrentType(int dwOutputStreamID, out IMFMediaType ppType)
        {
            ppType = null;
            HResult hr = HResult.S_OK;
            try
            {
                //DebugMessage("GetOutputCurrentType");
                CheckValidStream(dwOutputStreamID);
                lock (m_TransformLockObject)
                {
                    if (m_pOutputType != null)
                    {
                        ppType = WMFUtils.CloneMediaType(m_pOutputType);
                    }
                    else
                    {
                        // No output type set
                        hr = HResult.MF_E_TRANSFORM_TYPE_NOT_SET;
                    }
                }
            }
            catch (Exception e)
            {
                hr = (HResult)Marshal.GetHRForException(e);
            }
            return CheckReturn(hr);
        }

        /// <summary>
        /// Queries whether an input stream on this Transform can accept more data.
        ///
        /// </summary>
        /// <param name="dwInputStreamID">Input stream identifier. To get the list
        /// of stream identifiers, call IMFTransform::GetStreamIDs. </param>
        /// <param name="pdwFlags">Receives a member of the _MFT_INPUT_STATUS_FLAGS
        /// enumeration, or zero. If the value is MFT_INPUT_STATUS_ACCEPT_DATA,
        /// the stream specified in dwInputStreamID can accept more input data.</param>
        /// <returns>S_OK or other for fail</returns>
        public HResult GetInputStatus(int dwInputStreamID, out MFTInputStatusFlags pdwFlags)
        {
            pdwFlags = MFTInputStatusFlags.None;
            HResult hr = HResult.S_OK;
            try
            {
                //DebugMessage("GetInputStatus");
                CheckValidStream(dwInputStreamID);
                lock (m_TransformLockObject)
                {
                    if (CanAcceptInput())
                    {
                        pdwFlags = MFTInputStatusFlags.AcceptData;
                    }
                }
            }
            catch (Exception e)
            {
                hr = (HResult)Marshal.GetHRForException(e);
            }
            return CheckReturn(hr);
        }

        /// <summary>
        /// Queries whether the Transform (MFT) is ready to produce output data.
        ///
        /// </summary>
        /// <param name="pdwFlags">Receives a member of the _MFT_OUTPUT_STATUS_FLAGS
        /// enumeration, or zero. If the value is MFT_OUTPUT_STATUS_SAMPLE_READY,
        /// the MFT can produce an output sample.</param>
        /// <returns>S_OK or other for fail</returns>
        public HResult GetOutputStatus(out MFTOutputStatusFlags pdwFlags)
        {
            pdwFlags = MFTOutputStatusFlags.None;
            HResult hr = HResult.S_OK;
            try
            {
                //DebugMessage("GetOutputStatus");
                lock (m_TransformLockObject)
                {
                    if (HasPendingOutput())
                    {
                        pdwFlags = MFTOutputStatusFlags.SampleReady;
                    }
                }
            }
            catch (Exception e)
            {
                hr = (HResult)Marshal.GetHRForException(e);
            }
            return CheckReturn(hr);
        }

        /// <summary>
        /// Sets the range of time stamps the client needs for output.
        ///
        /// </summary>
        /// <param name="hnsLowerBound">Specifies the earliest time stamp.
        /// The Media Foundation transform (MFT) will accept input until it can produce an
        /// output sample that begins at this time; or until it can produce a sample that ends at
        /// this time or later. If there is no lower bound, use the value MFT_OUTPUT_BOUND_LOWER_UNBOUNDED. </param>
        /// <param name="hnsUpperBound">Specifies the latest time stamp. The MFT will not produce
        /// an output sample with time stamps later than this time. If there is no upper bound,
        /// use the value MFT_OUTPUT_BOUND_UPPER_UNBOUNDED. </param>
        /// <returns>S_OK or other for fail</returns>
        public HResult SetOutputBounds(long hnsLowerBound, long hnsUpperBound)
        {
            HResult hr = HResult.S_OK;
            try
            {
                //DebugMessage("SetOutputBounds");
                lock (m_TransformLockObject)
                {
                    // Output bounds not supported
                    hr = HResult.E_NOTIMPL;
                }
            }
            catch (Exception e)
            {
                hr = (HResult)Marshal.GetHRForException(e);
            }
            return CheckReturn(hr);
        }

        /// <summary>
        /// Sends an event to an input stream on this Transform.
        ///
        /// </summary>
        /// <param name="dwInputStreamID">Input stream identifier. To get the
        /// list of stream identifiers, call IMFTransform::GetStreamIDs. </param>
        /// <param name="pEvent">Pointer to the IMFMediaEvent interface of an
        /// event object. </param>
        /// <returns>S_OK or other for fail</returns>
        public HResult ProcessEvent(int dwInputStreamID, IMFMediaEvent pEvent)
        {
            HResult hr = HResult.S_OK;
            try
            {
                //DebugMessage("ProcessEvent");
                lock (m_TransformLockObject)
                {
                    // Events not supported.
                    hr = HResult.E_NOTIMPL;
                }
            }
            catch (Exception e)
            {
                hr = (HResult)Marshal.GetHRForException(e);
            }
            //finally
            {
                // MF.Net sample notes
                // While we *should* do this, we can't.  If the caller is c#, we
                // would destroy their copy.  Instead we have to leave this for
                // the GC.
                // SafeRelease(pEvent);
            }
            return CheckReturn(hr);
        }

        /// <summary>
        /// Sends a message to the Transform.
        ///
        /// </summary>
        /// <param name="eMessage">The message to send, specified as a
        /// member of the MFT_MESSAGE_TYPE enumeration.</param>
        /// <param name="ulParam">Message parameter. The meaning of this
        /// parameter depends on the message type. </param>
        /// <returns>S_OK or other for fail</returns>
        public HResult ProcessMessage(MFTMessageType eMessage, IntPtr ulParam)
        {
            HResult hr = HResult.S_OK;
            try
            {
                //DebugMessage("ProcessMessage " + eMessage.ToString());
                lock (m_TransformLockObject)
                {
                    switch (eMessage)
                    {
                        case MFTMessageType.NotifyStartOfStream:
                            // Optional message for non-async MFTs.
                            Reset();
                            break;

                        case MFTMessageType.CommandFlush:
                            Reset();
                            break;

                        case MFTMessageType.CommandDrain:
                            // Drain: Tells the MFT not to accept any more input until
                            // all of the pending output has been processed. That is our
                            // default behavior already, so there is nothing to do.
                            break;

                        case MFTMessageType.CommandMarker:
                            hr = HResult.E_NOTIMPL;
                            break;

                        case MFTMessageType.NotifyEndOfStream:
                            break;

                        case MFTMessageType.NotifyBeginStreaming:
                            break;

                        case MFTMessageType.NotifyEndStreaming:
                            break;

                        case MFTMessageType.SetD3DManager:
                            // The pipeline should never send this message unless the MFT
                            // has the MF_SA_D3D_AWARE attribute set to TRUE. However, if we
                            // do get this message, it's invalid and we don't implement it.
                            hr = HResult.E_NOTIMPL;
                            break;

                        default:
                            // DebugMessage("Unknown message type: " + eMessage.ToString());
                            // MF.Net sample comment
                            // The spec doesn't say to do this, but I do it anyway.
                            hr = HResult.S_FALSE;
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                hr = (HResult)Marshal.GetHRForException(e);
            }
            return CheckReturn(hr);
        }

        /// <summary>
        /// Delivers data to an input stream on this Transform. Since we usually do
        /// the actual work in ProcessOutput we just cache the input buffer. However
        /// we do call our virtual override so the child class can do some
        /// pre-processing if it wants to.
        ///
        /// </summary>
        /// <param name="dwFlags">Reserved. Must be zero. </param>
        /// <param name="dwInputStreamID">Input stream identifier. To get the
        /// list of stream identifiers, call IMFTransform::GetStreamIDs.</param>
        /// <param name="pSample">Pointer to the IMFSample interface of the input
        /// sample. The sample must contain at least one media buffer that contains
        /// valid input data. </param>
        /// <returns>S_OK or other for fail</returns>
        public HResult ProcessInput(int dwInputStreamID, IMFSample pSample, int dwFlags)
        {
            HResult hr;
            // some sanity checks
            hr = CheckValidStreamID(dwInputStreamID);
            if (hr != HResult.S_OK)
            {
                throw new Exception("ProcessInput call CheckValidStreamID on ID " + dwInputStreamID.ToString() + " failed. Err=" + hr.ToString());
            }
            // more checks
            if (pSample == null) return HResult.E_POINTER;
            // the docs state this must be zero
            if (dwFlags != 0) return HResult.E_INVALIDARG;
            lock (m_TransformLockObject)
            {
                hr = CheckIfInputAndOutputTypesAreSet();
                if (hr != HResult.S_OK)
                {
                    throw new Exception("ProcessInput call to CheckIfInputAndOutputTypesAreSet failed. Err=" + hr.ToString());
                }
                if (CanAcceptInput() == false) return HResult.MF_E_NOTACCEPTING;
                // Cache the sample. We usually do the actual
                // work in ProcessOutput, since that's when we
                // have the output buffer.
                m_pSample = pSample;
                // Call the virtual function.
                hr = OnProcessInput();
            }
            return CheckReturn(hr);
        }

        /// <summary>
        /// Generates output from the current input data. We expect InputSample
        /// to have been set. This is the source data.
        ///
        /// </summary>
        /// <param name="dwFlags">Bitwise OR of zero or more flags from the _MFT_PROCESS_OUTPUT_FLAGS enumeration. </param>
        /// <param name="cOutputBufferCount">Number of elements in the pOutputSamples array. The value must be at least 1. </param>
        /// <param name="pdwStatus">Receives a bitwise OR of zero or more flags from the _MFT_PROCESS_OUTPUT_STATUS enumeration.</param>
        /// <param name="outputSamplesArray">An array of MFT_OUTPUT_DATA_BUFFER structures, allocated by the caller. The MFT uses
        /// this array to return output data to the caller. One for each stream, however This base class is only
        /// designed for a single stream in and a single stream out. Thus we only
        /// process the first entry in the outputSamplesArray array</param>
        /// <returns>S_OK or other for fail</returns>
        public HResult ProcessOutput(MFTProcessOutputFlags dwFlags, int cOutputBufferCount, MFTOutputDataBuffer[] outputSamplesArray, out ProcessOutputStatus pdwStatus)
        {
            pdwStatus = ProcessOutputStatus.None;
            HResult hr = HResult.S_OK;
            // Check input parameters.
            if (cOutputBufferCount != 1) hr = HResult.E_INVALIDARG;
            if (dwFlags != MFTProcessOutputFlags.None) hr = HResult.E_INVALIDARG;
            if (outputSamplesArray == null) hr = HResult.E_POINTER;
            // In theory, we should check pOutputSamples[0].pSample,
            // but it may be null or not depending on how the derived
            // set MFTOutputStreamInfoFlags, so we leave the checking
            // for OnProcessOutput.
            lock (m_TransformLockObject)
            {
                hr = CheckIfInputAndOutputTypesAreSet();
                if (hr != HResult.S_OK)
                {
                    throw new Exception("ProcessOutput call to CheckIfInputAndOutputTypesAreSet failed. Err=" + hr.ToString());
                }
                // If we don't have an input sample, we need some input before
                // we can generate any output.
                if (HasPendingOutput() == false) return HResult.MF_E_TRANSFORM_NEED_MORE_INPUT;
                // This base class is only designed for a single stream in and a
                // single stream out. Thus we only process the Input Sample into
                // the first entry in the pOutputSamples array.
                hr = OnProcessOutput(ref outputSamplesArray[0]);
                if (hr != HResult.S_OK)
                {
                    throw new Exception("ProcessOutput call to OnProcessOutput failed. Err=" + hr.ToString());
                }
            }
            return HResult.S_OK;
        }

        // ########################################################################
        // ##### Private methods (Only expected to be used by template)
        // ########################################################################
        /// <summary>
        /// Allow for reset between NotifyStartOfStream calls.
        /// </summary>
        private void Reset()
        {
            InputSample = null;
            // Call the virtual function
            OnReset();
        }

        /// <summary>
        /// Are inputs allowed at the current time?
        /// </summary>
        /// <returns>true we can allow input, false we cannot</returns>
        private bool CanAcceptInput()
        {
            // If we already have an input sample, we don't accept
            // another one until the client calls ProcessOutput or Flush.
            if (m_pSample == null) return true;
            return false;
        }

        /// <summary>
        /// Do we have data for ProcessOutput?
        /// </summary>
        /// <returns>true we have data, false we do not</returns>
        private bool HasPendingOutput()
        {
            if (m_pSample != null) return true;
            return false;
        }

        /// <summary>
        /// Check for valid stream number.
        /// </summary>
        /// <param name="dwStreamID">Stream to check.</param>
        /// <remarks>Easy to do since the only valid value is zero.</remarks>
        private static void CheckValidStream(int dwStreamID)
        {
            // DebugTODO("remove this in the future");
            if (dwStreamID != 0)
            {
                throw new MFException(HResult.MF_E_INVALIDSTREAMNUMBER);
            }
        }

        /// <summary>
        /// Check for valid stream number. Easy to do since the only valid value is zero.
        /// </summary>
        /// <param name="dwStreamID">Stream to check.</param>
        /// <returns>S_OK or other for fail</returns>
        private static HResult CheckValidStreamID(int dwStreamID)
        {
            if (dwStreamID == 0) return HResult.S_OK;
            return HResult.MF_E_INVALIDSTREAMNUMBER;
        }

        /// <summary>
        /// Ensure both input and output media types are set.
        /// </summary>
        /// <returns>S_OK or other for fail</returns>
        private HResult CheckIfInputAndOutputTypesAreSet()
        {
            if (m_pInputType == null || m_pOutputType == null)
                return HResult.MF_E_TRANSFORM_TYPE_NOT_SET;
            return HResult.S_OK;
        }

        /// <summary>
        /// Print out a debug line when hr doesn't equal S_Ok.
        /// </summary>
        /// <param name="hr">Value to check</param>
        /// <returns>The input value.</returns>
        /// <remarks>This code shows the calling routine and the error text.
        /// All the public interface methods use this to wrap their returns.
        /// </remarks>
        private HResult CheckReturn(HResult hr)
        {
#if DEBUG
            if (hr != HResult.S_OK)
            {
                StackTrace st = new StackTrace();
                StackFrame sf = st.GetFrame(1);
                string sName = sf.GetMethod().Name;
                string sError = MFError.GetErrorText(hr);
                if (sError != null)
                    sError = sError.Trim();
                //DebugMessage(string.Format("{1} returning 0x{0:x} ({2})", hr, sName, sError));
            }
#endif
            return hr;
        }

        /// <summary>
        /// Gets/Sets the Input type. Note that setting a value here releases
        /// the previous value.
        /// </summary>
        protected IMFMediaType InputType
        {
            get { return m_pInputType; }
            set { SafeRelease(m_pInputType); m_pInputType = value; }
        }

        /// <summary>
        /// Gets/Sets the Output type. Note that setting a value here releases
        /// the previous value.
        /// </summary>
        protected IMFMediaType OutputType
        {
            get { return m_pOutputType; }
            set { SafeRelease(m_pOutputType); m_pOutputType = value; }
        }

        /// <summary>
        /// Gets/Sets the InputSample. Note that setting a value here releases
        /// the previous value.
        /// </summary>
        protected IMFSample InputSample
        {
            get { return m_pSample; }
            set { SafeRelease(m_pSample); m_pSample = value; }
        }
    }
}