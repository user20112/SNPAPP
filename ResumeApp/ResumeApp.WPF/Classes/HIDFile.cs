using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ResumeApp.WPF.Classes
{
    public class HIDFile : IDisposable
    {
        public const int COMMAND_PACKET_SIZE = 65;
        public const byte DLE = 0x10;
        public const byte EOT = 0x04;
        public const uint INVALID_HANDLE_VALUE = 0xFFFFFFFF;
        public const uint INVALID_SET_FILE_POINTER = 0xFFFFFFFF;
        public const byte SOH = 0x01;
        private bool _fDisposed;
        private string _sFileName = "";

        public HIDFile(string sFileName,
         DesiredAccess fDesiredAccess)
        {
            FileName = sFileName;
            Open(fDesiredAccess);
        }

        public HIDFile(string sFileName,
         DesiredAccess fDesiredAccess,
         CreationDisposition fCreationDisposition)
        {
            FileName = sFileName;
            Open(fDesiredAccess, fCreationDisposition);
        }

        public HIDFile(string sFileName,
            DesiredAccess fDesiredAccess,
            ShareMode fShareMode,
            CreationDisposition fCreationDisposition)
        {
            FileName = sFileName;
            Open(fDesiredAccess, fShareMode, fCreationDisposition, 0);
        }

        ~HIDFile()
        {
            Dispose(false);
        }

        public enum CreationDisposition : uint
        {
            CREATE_NEW = 1,
            CREATE_ALWAYS = 2,
            OPEN_EXISTING = 3,
            OPEN_ALWAYS = 4,
            TRUNCATE_EXSTING = 5
        }

        [Flags]
        public enum DesiredAccess : uint
        {
            GENERIC_READ = 0x80000000,
            GENERIC_WRITE = 0x40000000
        }

        [Flags]
        public enum FlagsAndAttributes : uint
        {
            FILE_ATTRIBUTES_ARCHIVE = 0x20,
            FILE_ATTRIBUTE_HIDDEN = 0x2,
            FILE_ATTRIBUTE_NORMAL = 0x80,
            FILE_ATTRIBUTE_OFFLINE = 0x1000,
            FILE_ATTRIBUTE_READONLY = 0x1,
            FILE_ATTRIBUTE_SYSTEM = 0x4,
            FILE_ATTRIBUTE_TEMPORARY = 0x100,
            FILE_FLAG_WRITE_THROUGH = 0x80000000,
            FILE_FLAG_OVERLAPPED = 0x40000000,
            FILE_FLAG_NO_BUFFERING = 0x20000000,
            FILE_FLAG_RANDOM_ACCESS = 0x10000000,
            FILE_FLAG_SEQUENTIAL_SCAN = 0x8000000,
            FILE_FLAG_DELETE_ON = 0x4000000,
            FILE_FLAG_POSIX_SEMANTICS = 0x1000000,
            FILE_FLAG_OPEN_REPARSE_POINT = 0x200000,
            FILE_FLAG_OPEN_NO_CALL = 0x100000
        }

        public enum MoveMethod : uint
        {
            FILE_BEGIN = 0,
            FILE_CURRENT = 1,
            FILE_END = 2
        }

        [Flags]
        public enum ShareMode : uint
        {
            FILE_SHARE_NONE = 0x0,
            FILE_SHARE_READ = 0x1,
            FILE_SHARE_WRITE = 0x2,
            FILE_SHARE_DELETE = 0x4,
        }

        public int FileLength
        {
            get
            {
                return (Handle != null) ? (int)GetFileSize(Handle,
                 IntPtr.Zero) : 0;
            }
            set
            {
                if (Handle == null)
                    return;
                MoveFilePointer(value, MoveMethod.FILE_BEGIN);
                if (!SetEndOfFile(Handle))
                    ThrowLastWin32Err();
            }
        }

        public string FileName
        {
            get { return _sFileName; }
            set
            {
                _sFileName = (value ?? "").Trim();
                if (_sFileName.Length == 0)
                    CloseHandle(Handle);
            }
        }

        public int FilePointer
        {
            get
            {
                return (Handle != null) ? (int)SetFilePointer(Handle, 0,
                 IntPtr.Zero, MoveMethod.FILE_CURRENT) : 0;
            }
            set
            {
                MoveFilePointer(value);
            }
        }

        public SafeFileHandle Handle { get; private set; } = null;
        public bool IsOpen { get { return (Handle != null); } }

        public void Close()
        {
            if (Handle == null)
                return;
            Handle.Close();
            Handle = null;
            _fDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public byte[] FormTXPacket(byte[] buff)
        {
            List<byte> TXPacket = new List<byte>
            {
                0x00,
                SOH
            };
            // Form TxPacket. Insert DLE in the data field whereever SOH and EOT are present.
            for (int i = 0; i < buff.Length; i++)
            {
                if ((buff[i] == EOT) || (buff[i] == SOH)
                        || (buff[i] == DLE))
                {
                    TXPacket.Add(DLE);
                }
                TXPacket.Add(buff[i]);
            }
            //Add the EOT: End of Transmission byte
            TXPacket.Add(EOT);
            //convert List to array for transmission
            return TXPacket.ToArray();
        }

        public void MoveFilePointer(int cbToMove)
        {
            MoveFilePointer(cbToMove, MoveMethod.FILE_CURRENT);
        }

        public void MoveFilePointer(int cbToMove,
         MoveMethod fMoveMethod)
        {
            if (Handle != null)
                if (SetFilePointer(Handle, cbToMove, IntPtr.Zero,
                 fMoveMethod) == INVALID_SET_FILE_POINTER)
                    ThrowLastWin32Err();
        }

        public void Open(
                                                 DesiredAccess fDesiredAccess)
        {
            Open(fDesiredAccess, CreationDisposition.OPEN_EXISTING);
        }

        public void Open(
         DesiredAccess fDesiredAccess,
         CreationDisposition fCreationDisposition)
        {
            ShareMode fShareMode;
            if (fDesiredAccess == DesiredAccess.GENERIC_READ)
            {
                fShareMode = ShareMode.FILE_SHARE_READ;
            }
            else
            {
                fShareMode = ShareMode.FILE_SHARE_NONE;
            }
            Open(fDesiredAccess, fShareMode, fCreationDisposition, 0);
        }

        public void Open(
         DesiredAccess fDesiredAccess,
         ShareMode fShareMode,
         CreationDisposition fCreationDisposition,
         FlagsAndAttributes fFlagsAndAttributes)
        {
            if (_sFileName.Length == 0)
                throw new ArgumentNullException("FileName");
            Handle = CreateFile(_sFileName, fDesiredAccess, fShareMode,
             IntPtr.Zero, fCreationDisposition, fFlagsAndAttributes,
             IntPtr.Zero);
            if (Handle.IsInvalid)
            {
                Handle = null;
                ThrowLastWin32Err();
            }
            _fDisposed = false;
        }

        public uint Read(byte[] buffer, uint cbToRead)
        {
            // returns bytes read
            uint cbThatWereRead = 0;
            if (!ReadFile(Handle, buffer, cbToRead,
             ref cbThatWereRead, IntPtr.Zero))
                ThrowLastWin32Err();
            return cbThatWereRead;
        }

        public byte[] ReadPIC32()
        {
            // Allocate memory and read
            byte[] bytes = new byte[COMMAND_PACKET_SIZE];
            Read(bytes, (uint)bytes.Length);
            return bytes;
        }

        public T ReadStructure<T>()
        {
            // Auto size if not specified
            uint size = (uint)Marshal.SizeOf(typeof(T));
            // Allocate memory and read
            byte[] bytes = new byte[size];
            Read(bytes, size);
            // Convert to target struct
            // Note that it does not copy the data, it assigns the struct to the same memory as the byte array.
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                T stuff = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
                return stuff;
            }
            finally
            {
                handle.Free();
            }
        }

        public uint Write(byte[] buffer, uint cbToWrite)
        {
            // returns bytes read
            uint cbThatWereWritten = 0;
            if (!WriteFile(Handle, buffer, cbToWrite,
             ref cbThatWereWritten, IntPtr.Zero))
                ThrowLastWin32Err();
            return cbThatWereWritten;
        }

        /// <summary>
        ///Write out Programming Data packet for Pic32, this will send multiple packets if needed
        /// This is needed because if we attempt to write the structure out we will get the wrong
        ///size and not all of the bytes in the data buffer. Instead it would return the size of the buffer
        ///contents which is not what we are looking for
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">The programming data bytes</param>
        /// <param name="CRC">CRC bytes</param>
        /// <param name="cmd">The data command if you wanted to override program (default)</param>
        public void WriteProgramPacket(byte[] data, UInt16 CRC, byte cmd)
        {
            List<byte> packet = new List<byte>
            {
                cmd
            };
            foreach (byte b in data)
            {
                packet.Add(b);
            }
            packet.Add((byte)CRC);
            packet.Add((byte)(CRC >> 8));
            byte[] array = FormTXPacket(packet.ToArray());
            if (array.Length > COMMAND_PACKET_SIZE) WriteMultiple(array);
            else Write(array, COMMAND_PACKET_SIZE);
        }

        public void WriteStructure<T>(T s, bool PIC32 = false)
        {
            // Auto size if not specified
            uint size = (uint)Marshal.SizeOf(typeof(T));
            // Convert struct to byte array
            byte[] bytes = new byte[size];
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                Marshal.StructureToPtr(s, handle.AddrOfPinnedObject(), false);
            }
            finally
            {
                handle.Free();
            }
            if (PIC32)
            {
                //Form the packet to send
                bytes = FormTXPacket(bytes);
            }
            // Write
            Write(bytes, COMMAND_PACKET_SIZE);
        }

        [DllImport("kernel32", SetLastError = true)]
        internal static extern Int32 CloseHandle(
         SafeFileHandle hObject);

        // Use interop to call the CreateFile function.
        // For more information about CreateFile,
        // see the unmanaged MSDN reference library.
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern SafeFileHandle CreateFile(
         string lpFileName,
         DesiredAccess dwDesiredAccess,
         ShareMode dwShareMode,
         IntPtr lpSecurityAttributes,
         CreationDisposition dwCreationDisposition,
         FlagsAndAttributes dwFlagsAndAttributes,
         IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern UInt32 GetFileSize(
         SafeFileHandle hFile,
         IntPtr pFileSizeHigh);

        [DllImport("kernel32", SetLastError = true)]
        internal static extern bool ReadFile(
         SafeFileHandle hFile,
         Byte[] aBuffer,
         UInt32 cbToRead,
         ref UInt32 cbThatWereRead,
         IntPtr pOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetEndOfFile(
         SafeFileHandle hFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern UInt32 SetFilePointer(
         SafeFileHandle hFile,
         Int32 cbDistanceToMove,
         IntPtr pDistanceToMoveHigh,
         MoveMethod fMoveMethod);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool WriteFile(
         SafeFileHandle hFile,
         Byte[] aBuffer,
         UInt32 cbToWrite,
         ref UInt32 cbThatWereWritten,
         IntPtr pOverlapped);

        protected virtual void Dispose(bool fDisposing)
        {
            if (!_fDisposed)
            {
                if (fDisposing)
                {
                    if (Handle != null)
                        Handle.Dispose();
                    _fDisposed = true;
                }
            }
        }

        private void ThrowLastWin32Err()
        {
            Marshal.ThrowExceptionForHR(
             Marshal.GetHRForLastWin32Error());
        }

        private uint WriteMultiple(byte[] buffer)
        {
            int dataToSend = buffer.Length;
            byte[] txPacket = new byte[COMMAND_PACKET_SIZE];
            uint cbThatWereWritten = 0; // returns bytes written
            uint totalcbThatWereWritten = 0; //stores the total bytes written
            uint cbToWrite = COMMAND_PACKET_SIZE;
            Array.Copy(buffer, txPacket, COMMAND_PACKET_SIZE);
            int count = 0; //counter for storing how many loops we make
            while (dataToSend > 0)
            {
                if (!WriteFile(Handle, txPacket, cbToWrite,
                ref cbThatWereWritten, IntPtr.Zero))
                    ThrowLastWin32Err();
                //update total counter
                totalcbThatWereWritten += cbThatWereWritten;
                //Subtract bytes written from the total bytes that need to be written
                dataToSend -= (int)cbThatWereWritten;
                if (dataToSend <= 0)
                {
                    break;
                }
                //clear array and store some more data
                Array.Clear(txPacket, 0, txPacket.Length);
                txPacket[0] = 0;
                if (dataToSend < COMMAND_PACKET_SIZE) Array.Copy(buffer, totalcbThatWereWritten - count, txPacket, 1, dataToSend);
                else Array.Copy(buffer, totalcbThatWereWritten - count, txPacket, 1, COMMAND_PACKET_SIZE - 1);
                dataToSend++; //we need to do this because we added a byte that was not in the orginal buffer to allow for transmission of the packet
                count++;
            }
            return totalcbThatWereWritten;
        }
    }
}