using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemReporting.Utilities.File
{
    /// <summary>
    /// Collection of files in a specified directory.
    /// </summary>
    public class Files : IEnumerable<string>
    {
        private string[] _files;

        internal Files(string path)
        {
            _files = Directory.GetFiles(path);
        }

        internal Files(string path, string searchPattern)
        {
            _files = Directory.GetFiles(path, searchPattern);
        }

        /// <summary>
        /// Returns enumerator for Name Value Pairs.
        /// </summary>
        /// <returns>Enumerator for Name Value Pairs.</returns>
        public IEnumerator<string> GetEnumerator()
        {

            foreach (string file in _files)
                yield return file;

        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Number of files.
        /// </summary>
        public int Count
        {

            get
            {
                int noFiles = 0;

                if (_files != null)
                    noFiles = _files.Length;

                return noFiles;
            }

        }

    }
}
