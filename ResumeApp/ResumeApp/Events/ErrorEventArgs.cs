using System;

namespace ResumeApp.Events
{
    public sealed class ErrorEventArgs : EventArgs
    {
        public int ErrorCode { get; set; }//  Error code returned
        public string Message { get; set; }//  message describing the error
    }
}