using System;

namespace ResumeApp.WPF.Classes.WMF
{
    public abstract class ObjBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ObjBase()
        {
        }

        /// <summary>
        /// Simple wrapper for the most common record message call
        /// </summary>
        /// <param name="msgText">Text to Write to the Log</param>
        public void LogMessage(string msgText)
        {
            // write it out to the log - but prepend the object type name
            Console.WriteLine(this.GetType().ToString() + ": " + msgText);
        }

        /// <summary>
        /// A wrapper to launch a modal Message box, with logging
        /// </summary>
        /// <param name="boxText">The text to display in the box</param>
        /// <param name="boxTitle">The box title</param>
        public void MessageBox(string boxText, string boxTitle)
        {
            if (boxTitle == null) boxTitle = "";
            if (boxText == null) boxText = "";
            LogMessage(boxTitle + " " + boxText);
            System.Windows.Forms.MessageBox.Show(boxText, boxTitle);
        }

        /// <summary>
        /// A wrapper to launch a modal Message box, with logging
        /// </summary>
        /// <param name="boxText">The text to display in the box</param>
        public void MessageBox(string boxText)
        {
            if (boxText == null) boxText = "";
            LogMessage(boxText);
            System.Windows.Forms.MessageBox.Show(boxText);
        }

        /// <summary>
        /// Simple wrapper for the most common debug mode record message call
        /// </summary>
        /// <param name="msgText">Text to Write to the Log</param>
    }
}