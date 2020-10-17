using MediaFoundation;
using System;
using System.Runtime.InteropServices;

namespace ResumeApp.WPF.Classes.WMF
{
    /// <summary>
    /// A class to act as a base class for COM objects in the  Library.
    /// Replaces COMBase in the MF.Net samples and inherits from OISBase
    /// so we can use its logging mechanisms.
    ///
    /// </summary>
    public class COMObjBase : ObjBase
    {
        /// <summary>
        /// A standard test for the failure of an HResult. Ported
        /// straight out of the MF.Net sample code COMBase class
        /// </summary>
        public static bool Failed(HResult hr)
        {
            return hr < 0;
        }

        /// <summary>
        /// A standard safe release of an object. Ported
        /// straight out of the MF.Net sample code COMBase class
        /// </summary>
        public static void SafeRelease(object o)
        {
            if (o != null)
            {
                if (Marshal.IsComObject(o))
                {
                    Marshal.ReleaseComObject(o);
                }
                else
                {
                    if (o is IDisposable iDis)
                    {
                        iDis.Dispose();
                    }
                    else
                    {
                        throw new Exception("SafeRelease: iDis != null");
                    }
                }
            }
        }

        /// <summary>
        /// A standard test for the success of an HResult. Ported
        /// straight out of the MF.Net sample code COMBase class
        /// </summary>
        public static bool Succeeded(HResult hr)
        {
            return hr >= 0;
        }
    }
}