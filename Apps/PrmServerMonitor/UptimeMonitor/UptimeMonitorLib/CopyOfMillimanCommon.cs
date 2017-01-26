using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UptimeMonitorLib
{
    /// <summary>
    /// Class containing copied code from MillimanCommon, due to unresolved dependencies in the MillimanCommon project
    /// </summary>
    class CopyOfMillimanCommon
    {
        public string AutoDecrypt(string Input, bool HexEncode = false)
        {
            string result = "";
            try
            {
                string PC = Input.Substring(0, 10);
                string ST = Input.Substring(Input.Length - 10, 10);
                string Data = Input.Substring(10, Input.Length - 20);
                string hashAlgorithm = "MD5";             // can be "SHA1"
                int passwordIterations = 2;                  // can be any number
                string initVector = "indyindyindyindy"; // must be 16 bytes
                int keySize = 128;                     // can be 192 or 128

                result = Decrypt(Data, PC, ST, hashAlgorithm, passwordIterations, initVector, keySize, HexEncode);

                //old version
                //System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
                //System.Text.Decoder utf8Decode = encoder.GetDecoder();
                //byte[] todecode_byte = Convert.FromBase64String(Input);
                //for (int i = 0; i < todecode_byte.Length - 1; i++)
                //{
                //    byte Tmp = todecode_byte[i + 1];
                //    todecode_byte[i + 1] = todecode_byte[i];
                //    todecode_byte[i] = Tmp;
                //}

                //int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
                //char[] decoded_char = new char[charCount];
                //utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
                //result = new String(decoded_char);
            }
            catch (Exception)
            {
                result = "";
                //Report.Log(Report.ReportType.Error, "Failed to encrypt data");

            }
            return result;
        }

        public string Decrypt(string cipherText,
                                     string passPhrase,
                                     string saltValue,
                                     string hashAlgorithm,
                                     int passwordIterations,
                                     string initVector,
                                     int keySize,
                                     bool HexEncode)
        {

            return SimpleDeCrypt(cipherText, HexEncode);
        }

        private string SimpleDeCrypt(string EncodedText, bool HexEncode = false)
        {
            //Base64 B64 = new Base64();
            //string ET = B64.Decode(EncodedText);
            //System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

            //byte[] Buffer = encoding.GetBytes(ET);
            if (HexEncode)
                EncodedText = ConvertHexToString(EncodedText);

            byte[] Buffer = System.Convert.FromBase64String(EncodedText);

            int Shift = 1;
            for (int i = 0; i < Buffer.Length; i++)
            {
                Buffer[i] = (byte)~Buffer[i];
                Buffer[i] = (byte)((char)Buffer[i] + (char)Shift);
                Shift++;
                if (Shift > 17)
                    Shift = 1;
            }

            string str;
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            str = enc.GetString(Buffer);

            return str;
        }

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

    }
}
