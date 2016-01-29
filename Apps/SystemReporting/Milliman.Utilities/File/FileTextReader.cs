using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milliman.Utilities.File
{
    /// <summary>
    /// This class wraps the most common ways of making calls to the StreamReader class.
    /// </summary>
    public class FileTextReader
    {
        private System.IO.StreamReader _sr;

        /// <summary>
        /// Constructor to initialize the FileTextReader object.
        /// </summary>
        /// <param name="path">Path to the file that will be read.</param>
        internal FileTextReader(string path)
        {
            _sr = new System.IO.StreamReader(path);
        }

        /// <summary>
        /// Determines if the reader is at the end of the file.
        /// </summary>
        public bool EOF
        {
            get
            {
                return (_sr.Peek() == -1);
            }
        }

        /// <summary>
        /// Read a single character
        /// </summary>
        /// <returns></returns>
        public int Read()
        {
            return _sr.Read();
        }

        /// <summary>
        /// Reads a certain number of characters into a buffer.
        /// </summary>
        /// <param name="buffer">The buffer to read the characters into</param>
        /// <param name="index">Starting index in the file</param>
        /// <param name="count">Number of characters to read</param>
        /// <returns></returns>
        public int Read(char[] buffer, int index, int count)
        {
            return _sr.Read(buffer, index, count);
        }

        /// <summary>
        /// Reads a line from the reader.
        /// </summary>
        /// <returns></returns>
        public string ReadLine()
        {
            return _sr.ReadLine();
        }

        /// <summary>
        /// Read all of the contents of the file to a string.
        /// </summary>
        /// <returns></returns>
        public string ReadToEnd()
        {
            return _sr.ReadToEnd();
        }

        /// <summary>
        /// Closes the reader object.
        /// </summary>
        public void Close()
        {
            _sr.Close();
        }
    }
}
