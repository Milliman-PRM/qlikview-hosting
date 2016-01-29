using SystemReporting.Utilities.ExceptionHandling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessor
{
    public sealed class DTHasher
    {
        public DTHasher() { }

        private static byte[] ConvertStringToByteArray(string data)
        {
            return (new System.Text.UnicodeEncoding()).GetBytes(data);
        }

        private static FileStream GetFileStream(string pathName)
        {
            return (new System.IO.FileStream(pathName, System.IO.FileMode.Open,
                                                        System.IO.FileAccess.Read,
                                                            System.IO.FileShare.ReadWrite));
        }

        public static string GetSHA1Hash(string pathName)
        {
            string strResult = "";
            string strHashData = "";

            byte[] arrbytHashValue;
            System.IO.FileStream oFileStream = null;

            SHA1CryptoServiceProvider oSHA1Hasher = new SHA1CryptoServiceProvider();

            try
            {
                oFileStream = GetFileStream(pathName);
                arrbytHashValue = oSHA1Hasher.ComputeHash(oFileStream);
                oFileStream.Close();

                strHashData = System.BitConverter.ToString(arrbytHashValue);
                strHashData = strHashData.Replace("-", "");
                strResult = strHashData;
            }
            catch (Exception ex)
            {
                SystemExceptionHandling.WriteException(ex);
            }

            return (strResult);
        }

        public static string GetMD5Hash(string pathName)
        {
            string strResult = "";
            string strHashData = "";

            byte[] arrbytHashValue;
            FileStream oFileStream = null;

            MD5CryptoServiceProvider oMD5Hasher = new MD5CryptoServiceProvider();

            try
            {
                oFileStream = GetFileStream(pathName);
                arrbytHashValue = oMD5Hasher.ComputeHash(oFileStream);
                oFileStream.Close();

                strHashData = System.BitConverter.ToString(arrbytHashValue);
                strHashData = strHashData.Replace("-", "");
                strResult = strHashData;
            }
            catch (Exception ex)
            {
                SystemExceptionHandling.WriteException(ex);
            }

            return (strResult);
        }

        public static String GetMD5HashFromFile(String fileName)
        {
            FileStream file = new FileStream(fileName, FileMode.Open);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(file);
            file.Close();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }

        public static String GetSHA1HashFromFile(String fileName)
        {
            FileStream file = new FileStream(fileName, FileMode.Open);
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] retVal = sha1.ComputeHash(file);
            file.Close();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }

        public string GetHash(string pathSrc, string dest)
        {
            //string pathDest = "copy_" + pathSrc;

            SystemReporting.Utilities.File.File file = new SystemReporting.Utilities.File.File();
            file.Copy(pathSrc, dest, true);

            String md5Result;
            StringBuilder sb = new StringBuilder();
            MD5 md5Hasher = MD5.Create();

            using (System.IO.FileStream fs = System.IO.File.OpenRead(dest))
            {
                foreach (Byte b in md5Hasher.ComputeHash(fs))
                    sb.Append(b.ToString("x2").ToLower());
            }

            md5Result = sb.ToString();

            file.Delete(dest);

            return md5Result;
        }

        public string GetHash(string pathSrc)
        {
            //https://www.youtube.com/watch?v=9MJAUL7G49w
            string filePath = pathSrc;
            byte[] buffer;
            int bytesRead = 0;
            long size = 0;
            long totalBytesRead = 0;

            String md5Result;
            using (Stream file = File.OpenRead(filePath))
            {
                size = file.Length;

                using (HashAlgorithm hasher = MD5.Create())
                {
                    do
                    {
                        buffer = new byte[4096];

                        bytesRead = file.Read(buffer, 0, buffer.Length);

                        totalBytesRead += bytesRead;
                        hasher.TransformBlock(buffer, 0, bytesRead, null, 0);
                    }
                    while (bytesRead != 0);

                    hasher.TransformFinalBlock(buffer, 0, 0);
                    md5Result = MakeHashString(hasher.Hash);

                }                
            }
            return md5Result;
        }

        private static string MakeHashString(byte[] hasBytes)
        {
            StringBuilder hash = new StringBuilder(32);

            foreach (byte b in hasBytes)
                hash.Append(b.ToString("X2").ToLower());

            return hash.ToString();
        }

    }
}
