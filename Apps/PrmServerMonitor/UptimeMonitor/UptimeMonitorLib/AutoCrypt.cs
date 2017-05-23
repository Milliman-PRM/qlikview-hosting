using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace MillimanCommon
{
    public class AutoCrypt
    {
        /// <summary>
        /// Create an autoencoded token for encryption test
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public string TokenAutoEncrypt(string Input)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] Buffer = encoding.GetBytes(Input);

            for (int i = 0; i < Buffer.Length; i++)
            {
                byte Temp = (byte)((char)Buffer[i] - (char)17);
                Buffer[i] = (byte)~Temp;
            }
            char[] hexDigits = { '0', '1', '2', '3', '4', '5', '6', '7',
                                 '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

            char[] chars = new char[Buffer.Length * 2];

            for (int i = 0; i < Buffer.Length; i++)
            {
                int b = Buffer[i];
                chars[i * 2] = hexDigits[b >> 4];
                chars[i * 2 + 1] = hexDigits[b & 0xF];
            }
            return new string(chars);
        }

        public string AutoEncrypt( string Input, bool HexEncode = false )
        {
            string encodedData = "";
            try
            {


                string GUID = System.Guid.NewGuid().ToString();
                GUID = GUID.Replace('-', '7');
                string PC = GUID.Substring(0, 10);
                string ST = GUID.Substring(10, 10);
                string hashAlgorithm = "MD5";             // can be "SHA1"
                int passwordIterations = 2;                  // can be any number
                string initVector = "indyindyindyindy"; // must be 16 bytes
                int keySize = 128;                     // can be 192 or 128

                encodedData = Encrypt(Input, PC, ST, hashAlgorithm, passwordIterations, initVector, keySize, HexEncode);

                encodedData = PC + encodedData + ST;
                //old version - increases length by 25%
                //byte[] encData_byte = new byte[Input.Length];
                //encData_byte = System.Text.Encoding.UTF8.GetBytes(Input);
                //for (int i = 0; i < encData_byte.Length - 1; i++)
                //{
                //    byte Tmp = encData_byte[i + 1];
                //    encData_byte[i + 1] = encData_byte[i];
                //    encData_byte[i] = Tmp;
                //}

                //encodedData = Convert.ToBase64String(encData_byte);

            }
            catch (Exception)
            {
                encodedData = "";
                //Report.Log(Report.ReportType.Error, "Failed to encrypt data");
            }
            return encodedData;

            
        }

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

        private string SimpleCrypt(string PlainText, bool HexEncode = false)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

            byte[] Buffer = encoding.GetBytes(PlainText);

            int Shift = 1;
            for (int i = 0; i < Buffer.Length; i++)
            {
                byte Temp = (byte)((char)Buffer[i] - (char)Shift);
                Buffer[i] = (byte)~Temp;
                Shift++;
                if (Shift > 17)
                    Shift = 1;
            }

            //Base64 B64 = new Base64();
            //string RetVal = B64.Encode(Buffer);
            //B64 = null;

            string RetVal = System.Convert.ToBase64String(Buffer);
            if (HexEncode)
                RetVal = ConvertStringToHex(RetVal);
            return RetVal;
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

        public string Encrypt(string plainText,
                                        string passPhrase,
                                        string saltValue,
                                        string hashAlgorithm,
                                        int passwordIterations,
                                        string initVector,
                                        int keySize,
                                        bool HexEncode )
        {

            return SimpleCrypt(plainText, HexEncode);

            //// Convert strings into byte arrays.
            //// Let us assume that strings only contain ASCII codes.
            //// If strings include Unicode characters, use Unicode, UTF7, or UTF8 
            //// encoding.
            //byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            //byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

            //// Convert our plaintext into a byte array.
            //// Let us assume that plaintext contains UTF8-encoded characters.
            //byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            //// First, we must create a password, from which the key will be derived.
            //// This password will be generated from the specified passphrase and 
            //// salt value. The password will be created using the specified hash 
            //// algorithm. Password creation can be done in several iterations.
            //PasswordDeriveBytes password = new PasswordDeriveBytes(
            //                                                passPhrase,
            //                                                saltValueBytes,
            //                                                hashAlgorithm,
            //                                                passwordIterations);

            //// Use the password to generate pseudo-random bytes for the encryption
            //// key. Specify the size of the key in bytes (instead of bits).
            //byte[] keyBytes = password.GetBytes(keySize / 8);

            //// Create uninitialized Rijndael encryption object.
            //RijndaelManaged symmetricKey = new RijndaelManaged();

            //// It is reasonable to set encryption mode to Cipher Block Chaining
            //// (CBC). Use default options for other symmetric key parameters.
            //symmetricKey.Mode = CipherMode.CBC;

            //// Generate encryptor from the existing key bytes and initialization 
            //// vector. Key size will be defined based on the number of the key 
            //// bytes.
            //ICryptoTransform encryptor = symmetricKey.CreateEncryptor(
            //                                                 keyBytes,
            //                                                 initVectorBytes);

            //// Define memory stream which will be used to hold encrypted data.
            //MemoryStream memoryStream = new MemoryStream();

            //// Define cryptographic stream (always use Write mode for encryption).
            //CryptoStream cryptoStream = new CryptoStream(memoryStream,
            //                                             encryptor,
            //                                             CryptoStreamMode.Write);
            //// Start encrypting.
            //cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);

            //// Finish encrypting.
            //cryptoStream.FlushFinalBlock();

            //// Convert our encrypted data from a memory stream into a byte array.
            //byte[] cipherTextBytes = memoryStream.ToArray();

            //// Close both streams.
            //memoryStream.Close();
            //cryptoStream.Close();

            //// Convert encrypted data into a base64-encoded string.
            //string cipherText = Convert.ToBase64String(cipherTextBytes);

            //// Return encrypted string.
            //return cipherText;
        }

        /// <summary>
        /// Decrypts specified ciphertext using Rijndael symmetric key algorithm.
        /// </summary>
        /// <param name="cipherText">
        /// Base64-formatted ciphertext value.
        /// </param>
        /// <param name="passPhrase">
        /// Passphrase from which a pseudo-random password will be derived. The
        /// derived password will be used to generate the encryption key.
        /// Passphrase can be any string. In this example we assume that this
        /// passphrase is an ASCII string.
        /// </param>
        /// <param name="saltValue">
        /// Salt value used along with passphrase to generate password. Salt can
        /// be any string. In this example we assume that salt is an ASCII string.
        /// </param>
        /// <param name="hashAlgorithm">
        /// Hash algorithm used to generate password. Allowed values are: "MD5" and
        /// "SHA1". SHA1 hashes are a bit slower, but more secure than MD5 hashes.
        /// </param>
        /// <param name="passwordIterations">
        /// Number of iterations used to generate password. One or two iterations
        /// should be enough.
        /// </param>
        /// <param name="initVector">
        /// Initialization vector (or IV). This value is required to encrypt the
        /// first block of plaintext data. For RijndaelManaged class IV must be
        /// exactly 16 ASCII characters long.
        /// </param>
        /// <param name="keySize">
        /// Size of encryption key in bits. Allowed values are: 128, 192, and 256.
        /// Longer keys are more secure than shorter keys.
        /// </param>
        /// <returns>
        /// Decrypted string value.
        /// </returns>
        /// <remarks>
        /// Most of the logic in this function is similar to the Encrypt
        /// logic. In order for decryption to work, all parameters of this function
        /// - except cipherText value - must match the corresponding parameters of
        /// the Encrypt function which was called to generate the
        /// ciphertext.
        /// </remarks>
        public  string Decrypt(string cipherText,
                                     string passPhrase,
                                     string saltValue,
                                     string hashAlgorithm,
                                     int passwordIterations,
                                     string initVector,
                                     int keySize,
                                     bool HexEncode )
        {

            return SimpleDeCrypt(cipherText, HexEncode);

            //// Convert strings defining encryption key characteristics into byte
            //// arrays. Let us assume that strings only contain ASCII codes.
            //// If strings include Unicode characters, use Unicode, UTF7, or UTF8
            //// encoding.
            //byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            //byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

            //// Convert our ciphertext into a byte array.
            //byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

            //// First, we must create a password, from which the key will be 
            //// derived. This password will be generated from the specified 
            //// passphrase and salt value. The password will be created using
            //// the specified hash algorithm. Password creation can be done in
            //// several iterations.
            //PasswordDeriveBytes password = new PasswordDeriveBytes(
            //                                                passPhrase,
            //                                                saltValueBytes,
            //                                                hashAlgorithm,
            //                                                passwordIterations);

            //// Use the password to generate pseudo-random bytes for the encryption
            //// key. Specify the size of the key in bytes (instead of bits).
            //byte[] keyBytes = password.GetBytes(keySize / 8);

            //// Create uninitialized Rijndael encryption object.
            //RijndaelManaged symmetricKey = new RijndaelManaged();

            //// It is reasonable to set encryption mode to Cipher Block Chaining
            //// (CBC). Use default options for other symmetric key parameters.
            //symmetricKey.Mode = CipherMode.CBC;

            //// Generate decryptor from the existing key bytes and initialization 
            //// vector. Key size will be defined based on the number of the key 
            //// bytes.
            //ICryptoTransform decryptor = symmetricKey.CreateDecryptor(
            //                                                 keyBytes,
            //                                                 initVectorBytes);

            //// Define memory stream which will be used to hold encrypted data.
            //MemoryStream memoryStream = new MemoryStream(cipherTextBytes);

            //// Define cryptographic stream (always use Read mode for encryption).
            //CryptoStream cryptoStream = new CryptoStream(memoryStream,
            //                                              decryptor,
            //                                              CryptoStreamMode.Read);

            //// Since at this point we don't know what the size of decrypted data
            //// will be, allocate the buffer long enough to hold ciphertext;
            //// plaintext is never longer than ciphertext.
            //byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            //// Start decrypting.
            //int decryptedByteCount = cryptoStream.Read(plainTextBytes,
            //                                           0,
            //                                           plainTextBytes.Length);

            //// Close both streams.
            //memoryStream.Close();
            //cryptoStream.Close();

            //// Convert decrypted data into a string. 
            //// Let us assume that the original plaintext string was UTF8-encoded.
            //string plainText = Encoding.UTF8.GetString(plainTextBytes,
            //                                           0,
            //                                           decryptedByteCount);

            //// Return decrypted string.   
            //return plainText;
        }

    }
}
