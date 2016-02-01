using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Diagnostics;

namespace MillimanCommon
{
    public class HtmlConversion
    {
        public enum CovertTo { PDF, IMAGE };
        static public bool CovertHTML(string HTMLFile, out string OutputFileName, string RequestedOutputFile = "", CovertTo CovertToType = CovertTo.IMAGE)
        {
            OutputFileName = "";
            try
            {
                string Converter = ConfigurationManager.AppSettings["HTMLtoPDFConverter"];
                if ( CovertToType == CovertTo.IMAGE )
                    Converter = ConfigurationManager.AppSettings["HTMLtoImageConverter"];

                if (System.IO.File.Exists(Converter) && System.IO.File.Exists(HTMLFile))
                {
                    if (string.IsNullOrEmpty(RequestedOutputFile) == true)  //user specified a file
                    {
                        RequestedOutputFile = System.IO.Path.GetDirectoryName(HTMLFile) + "\\" + System.IO.Path.GetFileNameWithoutExtension(HTMLFile);
                        if (CovertToType == CovertTo.PDF)
                            RequestedOutputFile += ".pdf";
                        else
                            RequestedOutputFile += ".png";
                    }

                    OutputFileName = RequestedOutputFile;
                    System.IO.File.Delete(RequestedOutputFile);

                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.UseShellExecute = false;
                    psi.FileName = Converter;
                    psi.CreateNoWindow = true;

                    psi.Arguments = "-q \"" + HTMLFile + "\" \"" + RequestedOutputFile + "\"";
                    Process p = Process.Start(psi);

                    p.WaitForExit(60000); //wait max 1 minute

                    return System.IO.File.Exists(RequestedOutputFile);
                }
            }
            catch (Exception)
            {
            }
            return false;
        }

    }
}
