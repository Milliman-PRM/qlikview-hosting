using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemReporting.Utilities.File
{
    /// <summary>
    /// This class wraps the most common ways of making calls to the StreamWriter object.
    /// </summary>
    public class FileTextWriter
    {
        private System.IO.StreamWriter _sw;

        /// <summary>
        /// Constructor to initialize the FileTextWriter object.
        /// </summary>
        /// <param name="path"></param>
        internal FileTextWriter(string path)
        {
            _sw = new System.IO.StreamWriter(path);
        }

        /// <summary>
        /// Writes data to the file (this does not automatically add a new line to the file).
        /// </summary>
        /// <param name="value"></param>
        public void Write(string value)
        {
            _sw.Write(value);
        }

        /// <summary>
        /// Writes a line to the file.
        /// </summary>
        /// <param name="value"></param>
        public void WriteLine(string value)
        {
            _sw.WriteLine(value);
        }

        /// <summary>
        /// Close the file
        /// </summary>
        public void Close()
        {
            _sw.Close();
        }
    }
}
