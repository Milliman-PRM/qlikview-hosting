using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Polenter.Serialization;


namespace MillimanCommon
{
    public class DownloadDescriptions
    {
        public string DownloadItem { get; set; }
        public string IconFile { get; set; }
        public string Description { get; set; }

        public DownloadDescriptions()
        {

        }

        /// <summary>
        /// Get the directory name of the custom downloads for a QVW or project
        /// </summary>
        /// <param name="QVWorPRJQualifiedPath"></param>
        /// <returns></returns>
        static public string GetDownloadsDirectory( string QVWorPRJQualifiedPath )
        {
            string ProcessedDir = string.Empty;
            if ( QVWorPRJQualifiedPath.ToLower().Contains(".hciprj") )
                ProcessedDir = QVWorPRJQualifiedPath.ToLower().Replace(".hciprj","_Data");
            else if ( QVWorPRJQualifiedPath.ToLower().Contains(".qvw") )
                ProcessedDir = QVWorPRJQualifiedPath.ToLower().Replace(".qvw","_Data");

           // ProcessedDir = ProcessedDir.Replace(' ', '_');
            return ProcessedDir;
        }

        /// <summary>
        /// Get the name of the description object for the item
        /// </summary>
        /// <param name="QualifiedItemName"></param>
        /// <returns></returns>
        static public string GetDescriptionFilename(string QualifiedItemName)
        {
            string NewFilename = System.IO.Path.GetFileNameWithoutExtension(QualifiedItemName) + ".description";
            return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(QualifiedItemName), NewFilename);
        }
        static public DownloadDescriptions Load(string FileAndPath)
        {
            try
            {
                var serializer = new SharpSerializer(false);
                if (System.IO.File.Exists(FileAndPath))
                {
                    DownloadDescriptions DD = serializer.Deserialize(FileAndPath) as DownloadDescriptions;
                    return DD;
                }
            }
            catch (Exception ex)
            {
                Report.Log(Report.ReportType.Error, "DownloadDescriptions:GetDescriptionFilename|----Sharp serializer failed----");
            }
            return null;
        }

        public bool Save( string FileAndPath )
        {
            try
            {
                var serializer = new SharpSerializer(false);
                serializer.Serialize(this, FileAndPath);
                return true;
            }
            catch (Exception ex)
            {
                MillimanCommon.Report.Log(Report.ReportType.Error, "DownloadDescriptions:Save|----Failed to save file----", ex);
            }
            return false;
        }

        static public string AllDescriptionsToXML(string QVWorPRJQualifiedName)
        {
            string ProjectName = QVWorPRJQualifiedName;
            if (ProjectName.ToLower().Contains(".qvw"))
                ProjectName = ProjectName.ToLower().Replace(".qvw", ".hciprj");

            MillimanCommon.ProjectSettings PS = MillimanCommon.ProjectSettings.Load(ProjectName);

            string Root = "<Node Expanded=\"True\" Text=\"" + System.IO.Path.GetFileNameWithoutExtension(QVWorPRJQualifiedName) + "\" Tooltip=\"" + PS.QVDescription + "\"  Value=\"_VALUE_\" ImageURL=\"_IMAGE_\" >";
            Root = Root.Replace("_VALUE_", ProjectName.ToLower().Replace(".hciprj", ".qvw"));
            Root = Root.Replace("_IMAGE_", "images/rootreport.gif");
            string NodeXML = "<Node Text=\"_TEXT_\" Value=\"_VALUE_\" Tooltip=\"_TOOLTIP_\" ImageURL=\"_IMAGE_\" ></Node>";
            string DownloadDirectory = DownloadDescriptions.GetDownloadsDirectory(QVWorPRJQualifiedName);
            if (System.IO.Directory.Exists(DownloadDirectory) == true)
            {
                string[] DescriptionFiles = System.IO.Directory.GetFiles(DownloadDirectory, "*.description");
                foreach (string DescriptionFile in DescriptionFiles)
                {
                    DownloadDescriptions DD = DownloadDescriptions.Load(DescriptionFile);
                    string DDNode = NodeXML.Replace("_TEXT_", DD.DownloadItem);
                    DDNode = DDNode.Replace("_VALUE_", System.IO.Path.Combine(DownloadDirectory, DD.DownloadItem));
                    DDNode = DDNode.Replace("_TOOLTIP_", DD.Description);

                    string Mime = string.Empty;
                    string FullIconPath = MillimanCommon.ImageUtilities.GetIcon(System.IO.Path.Combine(DownloadDirectory, DD.DownloadItem), out Mime);
                    //string FullIconPath = System.IO.Path.Combine(DownloadDirectory, DD.IconFile);
                    //FullIconPath = MillimanCommon.Utilities.ConvertStringToHex(FullIconPath);
                    //DDNode = DDNode.Replace("_IMAGE_", "ImageReflector.aspx?Key=" + FullIconPath);
                    DDNode = DDNode.Replace("_IMAGE_", FullIconPath);
                    Root += DDNode;
                }
            }
            return Root + "</Node>";
        }
    }
}
