using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MillimanCommon
{
    public class FileSignature
    {
        const string Header = "HCINTEL";

        public bool IsSigned(string QualifiedFile)
        {
            if (System.IO.File.Exists(QualifiedFile) == false)
                return false;
            System.IO.FileInfo FI = new System.IO.FileInfo(QualifiedFile);

            //read the trailer to see how long the xml sig is
            byte[] SignatureLength = ReadSection(QualifiedFile, (int)FI.Length - 32, 32);
            string sSignatureLength = MillimanCommon.Utilities.ByteArrayToString(SignatureLength);

            return sSignatureLength.IndexOf(Header) != -1;
        }

        public bool SignFile(string QualifiedFile, string Signature)
        {
            if (System.IO.File.Exists(QualifiedFile) == false)
                return false;

            int SigLength = Signature.Length;
            string sSigLength = SigLength.ToString("D32");//always 32 chars
            //add the header
            sSigLength = Header + sSigLength.Substring(Header.Length);
            Signature += sSigLength;
            byte[] bSignature = MillimanCommon.Utilities.StringToByteArray(Signature);

            //remove old signature if present
            RemoveSignature(QualifiedFile);
            //check to see if file only contained signature
            System.IO.FileStream writeStream;
            try
            {
                writeStream = new System.IO.FileStream(QualifiedFile, System.IO.FileMode.Append, System.IO.FileAccess.Write);
                System.IO.BinaryWriter writeBinay = new System.IO.BinaryWriter(writeStream);
                writeBinay.Write(bSignature);
                writeBinay.Close();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public byte[] ReadSection(string QualifiedFile, int FromPos, int Length)
        {
            if (System.IO.File.Exists(QualifiedFile) == false)
                return null;

            byte[] Data = new byte[Length];
            using (System.IO.FileStream sr = System.IO.File.OpenRead(QualifiedFile))
            {
                sr.Seek(FromPos, System.IO.SeekOrigin.Begin);

                sr.Read(Data, 0, Length);
            }
            return Data;
        }

        public string GetSignature(string QualifiedFile)
        {
            if (System.IO.File.Exists(QualifiedFile) == false)
                return "";
            System.IO.FileInfo FI = new System.IO.FileInfo(QualifiedFile);

            //read the trailer to see how long the xml sig is
            byte[] SignatureLength = ReadSection(QualifiedFile, (int)FI.Length - 32, 32);
            string sSignatureLength = MillimanCommon.Utilities.ByteArrayToString(SignatureLength);
            if (sSignatureLength.IndexOf(Header) == -1)
                return "";  //not a valid signature
            int SigLength = System.Convert.ToInt32(sSignatureLength.Substring(Header.Length));

            byte[] bSignature = ReadSection(QualifiedFile, (int)FI.Length - (SigLength + 32), SigLength);

            return MillimanCommon.Utilities.ByteArrayToString(bSignature);
        }

        public bool RemoveSignature(string QualifiedFile)
        {
            if (System.IO.File.Exists(QualifiedFile) == false)
                return false;
            System.IO.FileInfo FI = new System.IO.FileInfo(QualifiedFile);

            //read the trailer to see how long the xml sig is
            byte[] SignatureLength = ReadSection(QualifiedFile, (int)FI.Length - 32, 32);
            string sSignatureLength = MillimanCommon.Utilities.ByteArrayToString(SignatureLength);
            if (sSignatureLength.IndexOf(Header) == -1)
                return true;  //didn't really have a signature
            int SigLength = System.Convert.ToInt32(sSignatureLength.Substring(Header.Length));

            System.IO.FileStream fs = FI.Open(System.IO.FileMode.Open);
            fs.SetLength(Math.Max(0, FI.Length - (SigLength + 32))); //header is 32 bytes

            fs.Close();

            return true;
        }

    }
}
