using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MillimanCommon
{
    public class QVWUtilities
    {
        /// <summary>
        /// Take a fully qualified QVW path and turn in  a virtual from QV Documents root
        /// </summary>
        /// <param name="AbsolutePath"></param>
        /// <returns></returns>
        static public string AbsoluteToVirtualFromQVDocRoot(string AbsolutePath)
        {
            string QVDocumentRoot = System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"];
            if ( AbsolutePath.Contains(':') == false )
                return AbsolutePath;   //no colon,  can't be absolute

            AbsolutePath = AbsolutePath.Substring(QVDocumentRoot.Length);
            AbsolutePath = AbsolutePath.Replace('/', '\\');
            if (AbsolutePath.StartsWith("\\"))
                AbsolutePath = AbsolutePath.Substring(1);
            return AbsolutePath;
        }

        /// <summary>
        /// Take a virtual path relative to QV Documents and make absolute to QV Documents
        /// </summary>
        /// <param name="VirtualPath"></param>
        /// <returns></returns>
        static public string VirtualToAbsoluteFromQVDocRoot(string VirtualPath)
        {
            string QVDocumentRoot = System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"];
            if (VirtualPath.Contains(':') == true)
                return VirtualPath;   //contains a colon, must already be absolute

            VirtualPath = VirtualPath.Replace('/', '\\');
            if (VirtualPath.StartsWith("\\"))
                VirtualPath = VirtualPath.Substring(1);

            return System.IO.Path.Combine(QVDocumentRoot, VirtualPath);
        }
    }
}
