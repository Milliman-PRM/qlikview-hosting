using System;
using System.IO;

namespace SystemReporting.Utilities.File
{
    /// <summary>
    /// File interface.  All file operations are performed though this class.
    /// </summary>
    public class File
    {
        public File() {

        }

        public static void MakeDirectory(string dirPath)
        {
            System.IO.Directory.CreateDirectory(dirPath);
        }

        public static bool DirectoryExists(string dirPath)
        {
            return System.IO.Directory.Exists(dirPath);
        }

        /// <summary>
        /// Used to create a new file through the FileTextWriter object.
        /// </summary>
        /// <param name="path">Path for the new file</param>
        /// <returns>FileTextWriter object.</returns>
        public static FileTextWriter CreateFile(string path)
        {
            return new FileTextWriter(path);
        }

        /// <summary>
        /// Used to read a file through the FileTextReader object.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static FileTextReader ReadFile(string path)
        {
            return new FileTextReader(path);
        }

        /// <summary>
        /// Collection of files in a specified directory.
        /// </summary>
        /// <param name="path">The directory to find files in.</param>
        /// <returns>Returns a collection of files based on the given directory.</returns>
        public static Files GetFiles(string path)
        {
            return new Files(path);
        }

        /// <summary>
        /// Collection of files in a specified directory
        /// </summary>
        /// <param name="path">The directory to find files in.</param>
        /// <param name="searchPattern">Filter for a search, such as *.txt</param>
        /// <returns>Returns a collection of files based on the given directory.</returns>
        public static Files GetFiles(string path, string searchPattern)
        {
            return new Files(path, searchPattern);
        }

        /// <summary>
        /// Checks for the existence of a file
        /// </summary>
        /// <param name="path">The file path to check</param>
        /// <returns>Boolean</returns>
        public static bool Exists(string fileFullPath)
        {
            return System.IO.File.Exists(fileFullPath);
        }

        /// <summary>
        /// Deletes a specified file
        /// </summary>
        /// <param name="path">The file to delete</param>
        public static void Delete(string path)
        {
            System.IO.File.Delete(path);
        }

        /// <summary>
        /// Moves a file from one location to another
        /// </summary>
        /// <param name="sourceFileName"></param>
        /// <param name="destFileName"></param>
        public static void Move(string sourceFileName, string destFileName)
        {
            System.IO.File.Move(sourceFileName, destFileName);
        }

        public static void CreateDirectory(string directoryPath)
        { 
            System.IO.Directory.CreateDirectory(directoryPath); 
        }
                
        /// <summary>
        /// Gets the name of a file from a path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFileName(string path)
        {
            return System.IO.Path.GetFileName(path);
        }

        public static string GetFileNameWithoutExtension(string filePath) { return System.IO.Path.GetFileNameWithoutExtension(filePath); }

        /// <summary>
        /// Copies a file from one location to another.
        /// </summary>
        /// <param name="sourceFileName"></param>
        /// <param name="destFileName"></param>
        public static void Copy(string sourceFileName, string destFileName)
        {
            System.IO.File.Copy(sourceFileName, destFileName);
        }

        public static void Copy(string sourceFilePath, string destinationFilePath, bool overWrite)
        {
            var whatwasInfoReadOnly = false;
            var fileExists = false;
            System.IO.FileInfo info;

            if (Exists(destinationFilePath) && overWrite)
            {
                fileExists = true;
                info = new System.IO.FileInfo(destinationFilePath);
                if (info.IsReadOnly)
                {
                    whatwasInfoReadOnly = info.IsReadOnly;
                    info.IsReadOnly = false;
                }
            }

            System.IO.File.Copy(sourceFilePath, destinationFilePath, overWrite);

            if (fileExists)
            {
                info = new System.IO.FileInfo(destinationFilePath);
                info.IsReadOnly = whatwasInfoReadOnly;
            }
        }

        
        public static void FilesCopy(string sourceDirectoryFilePath, string destinationDirectoryFilePath, bool overWrite)
        {                     
             Copy(sourceDirectoryFilePath, destinationDirectoryFilePath, overWrite);
        }

        public static string Combine(params string[] paths)
        {
            var combinedPath = String.Empty;
            foreach (string path in paths) { combinedPath = System.IO.Path.Combine(combinedPath, path); }
            return combinedPath;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sResults"></param>
        /// <param name="filFullNamePath"></param>
        public static bool WriteFile(string sResults, string filFullNamePath)
        {
            //create the file name
            FileTextWriter textWriter = File.CreateFile(filFullNamePath);

            var outputValue = sResults;
            textWriter.WriteLine(outputValue);
            textWriter.Close();

            //if the file is there then
            var returnValue = false;
            if (outputValue.Length > 0)
                returnValue = true;

            return returnValue;
        }
        #region Copy File Funciton

        public static void CopyFile(string source, string destination)
        {
            var sourceFile = new FileInfo(source);
            var destFile = new FileInfo(destination);

            CopyFile(sourceFile, destFile);
        }

        public static void CopyFile(FileInfo source, FileInfo destination)
        {
            CopyFile(source, destination, true, true);
        }

        public static void CopyFile(FileInfo source, FileInfo destination, bool preserveFileDate, bool overwriteIfExists)
        {
            //check source exists
            if (!source.Exists)
            {
                var message = "File '" + source.FullName + "' does not exist";
                throw new FileNotFoundException(message);
            }

            //does destinations directory exist ?
            if (destination.Directory != null && !destination.Directory.Exists)
            {
                destination.Directory.Create();
            }

            //make sure we can write to destination
            if (destination.Exists && (destination.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                var message = "Unable to open file '" + destination.FullName + "' for writing.";
                throw new IOException(message);
            }

            //makes sure it is not the same file        
            if (source.DirectoryName.Equals(destination.DirectoryName))
            {
                var message = "Unable to write file '" + source + "' on itself.";
                throw new IOException(message);
            }

            File.CopyFile(source.FullName, destination.FullName);

            destination.Refresh();

            if (source.Length != destination.Length)
            {
                var message =
                  "Failed to copy full contents from "
                  + source.FullName
                  + " to "
                  + destination.FullName;
                throw new IOException(message);
            }

            if (preserveFileDate)
            {
                //file copy should preserve file date
                destination.LastWriteTime = source.LastWriteTime;
            }
        }

        public static void CopyFiles(string sourcePath, string destinationPath)
        {
            CopyFiles(new DirectoryInfo(sourcePath), new DirectoryInfo(destinationPath));
        }

        public static void CopyFiles(DirectoryInfo source, DirectoryInfo destination)
        {

            if (!source.Exists)
                throw new ArgumentException(string.Format("Source directory '{0}' does not exist", source));

            if (!destination.Exists)
                destination.Create();

            FileInfo[] sourceFiles = source.GetFiles();
            foreach (FileInfo file in sourceFiles)
            {
                file.CopyTo(destination.FullName + Path.DirectorySeparatorChar + file.Name);
            }

            DirectoryInfo[] subdirs = source.GetDirectories();
            foreach (DirectoryInfo subdir in subdirs)
            {
                DirectoryInfo destSubDir = Directory.CreateDirectory(destination.FullName + Path.DirectorySeparatorChar + subdir.Name);
                CopyFiles(subdir, destSubDir);
            }
        }
        #endregion
    }
}
