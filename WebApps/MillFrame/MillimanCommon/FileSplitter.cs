using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MillimanCommon
{
    /// <summary>
    /// Splits a file into multiple parts per specified parameters
    /// and optionally creates a batch job to re-assemble parts
    /// </summary>
    public class FileSplitter
    {
        /// <summary>
        /// Method to split a file into sections, typically required for transport when file size limits
        /// are imposed
        /// </summary>
        /// <param name="QualifiedFileWithPath">Qualfied path and filename to split</param>
        /// <param name="FilePartSize">Max size an individual size can be</param>
        /// <param name="NumberParts"> Methods returns the number of parts created from ([file size]/FilePartSize) + 1 </param>
        /// <param name="CreateAssemblyBatch">Creates a MS batch file that when run will re-assemble the parts to the original</param>
        /// <param name="WritePartsToDirectory">Write parts and batch to this optional directory, otherwise write it to path of input file</param>
        /// <returns></returns>
        public static bool SplitFile(string QualifiedFileWithPath, int FilePartSize, out int NumberParts, bool CreateAssemblyBatch = true, string WritePartsToDirectory = "")
        {
            NumberParts = 0;
            try
            {
                if (System.IO.File.Exists(QualifiedFileWithPath) == false)
                {
                    MillimanCommon.Report.Log(Report.ReportType.Error, "File:'" + QualifiedFileWithPath + "' does not exist to split");
                    return false;
                }
                System.IO.FileStream FR = new System.IO.FileStream(QualifiedFileWithPath, System.IO.FileMode.Open, System.IO.FileAccess.Read);

                int intByteSize = FilePartSize;
                //Check if split size is greater than or equal to actual file size
                if (intByteSize >= FR.Length)
                {
                    MillimanCommon.Report.Log(Report.ReportType.Error, "Split size is greater than or equal to the actual file size");
                    FR.Close();
                    return false;
                }

                //Calculate the number of parts the file should be splitted
                NumberParts = Int32.Parse(FR.Length.ToString()) / intByteSize;

                int intmod = Int32.Parse(FR.Length.ToString()) % intByteSize;

                //Include the remaining bytes
                if (intmod > 0)
                    NumberParts = NumberParts + 1;

                byte[] by = null;
                string filename = System.IO.Path.GetFileName(QualifiedFileWithPath);
                string dirpath = System.IO.Path.GetDirectoryName(QualifiedFileWithPath);
                string Batch_Parameters = null;

                string WritePartsTo = QualifiedFileWithPath;
                if (string.IsNullOrEmpty(WritePartsToDirectory) == false)
                {
                    WritePartsTo = System.IO.Path.Combine( WritePartsToDirectory, System.IO.Path.GetFileName(QualifiedFileWithPath));
                    dirpath = WritePartsToDirectory;
                }
                //Read the file by number of bytes and create part files
                for (int i = 0; i < NumberParts; i++)
                {
                    if (intmod > 0 && i == NumberParts - 1)
                    {
                        by = new byte[intmod];
                        FR.Read(by, 0, intmod);
                    }
                    else
                    {
                        by = new byte[intByteSize];
                        FR.Read(by, 0, intByteSize);
                    }
                    // Write split files and create temp file names
                    System.IO.FileStream FW = new System.IO.FileStream(WritePartsTo + "_" + i.ToString(), System.IO.FileMode.Create, System.IO.FileAccess.Write);
                    using (FW)
                    {
                        if (Batch_Parameters != null)
                            Batch_Parameters = Batch_Parameters + "+\"" + filename + "_" + i.ToString() + "\" ";
                        else
                            Batch_Parameters = Batch_Parameters + "\"" + filename + "_" + i.ToString() + "\" ";
                        foreach (byte b in by)
                        {
                            FW.WriteByte(b);
                        }
                    }
                }
                if (CreateAssemblyBatch)
                {
                    //Create batch file if requested
                    System.IO.StreamWriter SW = new System.IO.StreamWriter(dirpath + "\\" + System.IO.Path.GetFileNameWithoutExtension(QualifiedFileWithPath) + ".bat");
                    using (SW)
                    {
                        StringBuilder SB = new StringBuilder();
                        SB.Append("Copy /b " + Batch_Parameters + " \"" + filename + "\"");
                        SW.Write(SB.ToString());
                        FR.Close();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MillimanCommon.Report.Log(Report.ReportType.Error, ex.ToString());
            }
            return false;
        }
    }
}
