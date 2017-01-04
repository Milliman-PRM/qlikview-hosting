using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MillimanCommon
{
    public static class Utilities
    {
        public static string ConvertStringToHex(string asciiString)
        {
            string hex = "";
            foreach (char c in asciiString)
            {
                int tmp = c;
                hex += String.Format("{0:x2}", (uint)System.Convert.ToUInt32(tmp.ToString()));
            }
            return hex;
        }

        public static string ConvertHexToString(string HexValue)
        {
            string StrValue = "";
            while (HexValue.Length > 0)
            {
                StrValue += System.Convert.ToChar(System.Convert.ToUInt32(HexValue.Substring(0, 2), 16)).ToString();
                HexValue = HexValue.Substring(2, HexValue.Length - 2);
            }
            return StrValue;
        }

        public static byte[] StringToByteArray(string InputString)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(InputString);

            //byte[] bytes = new byte[InputString.Length * sizeof(char)];
            //System.Buffer.BlockCopy(InputString.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
        public static string ByteArrayToString(byte[] bytes)
        {
            string result = System.Text.Encoding.UTF8.GetString(bytes);
            return result;
            //char[] chars = new char[bytes.Length / sizeof(char)];
            //System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            //return new string(chars);
        }
        public static string CalculateMD5Hash(string input, bool isFileName = false)
        {
            byte[] inputBytes = null;
            if (isFileName == false)
            {
                inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            }
            else
            {
                if (System.IO.File.Exists(input) && input.Length != 0)
                    inputBytes = System.IO.File.ReadAllBytes(input);
                else
                    return "";
            }

            // step 1, calculate MD5 hash from input
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();

            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public static long GetDirectorySize(string p)
        {
            long b = 0;
            if (System.IO.Directory.Exists(p))
            {
                string[] a = System.IO.Directory.GetFiles(p, "*.*", System.IO.SearchOption.AllDirectories);

                foreach (string name in a)
                {
                    System.IO.FileInfo info = new System.IO.FileInfo(name);
                    b += info.Length;
                }
            }
            return b;
        }

        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs, List<string> Wildcards = null )
        {
            if (Wildcards == null)
                Wildcards = new List<string>() { "*.*" };

            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            foreach (string FileType in Wildcards)
            {
                FileInfo[] files = dir.GetFiles(FileType);
                foreach (FileInfo file in files)
                {
                    string temppath = Path.Combine(destDirName, file.Name);
                    file.CopyTo(temppath, false);
                }
            }
            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs, Wildcards);
                }
            }
        }
    }
}
