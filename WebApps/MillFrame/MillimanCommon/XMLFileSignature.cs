using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MillimanCommon
{
    public class XMLFileSignature
    {
        private Dictionary<string, string> _SignatureDictionary = new Dictionary<string, string>();
        public Dictionary<string, string> SignatureDictionary
        {
            get { return _SignatureDictionary; }
            set { _SignatureDictionary = value; }
        }

        private string _ErrorMessage;

        public string ErrorMessage
        {
            get { return _ErrorMessage; }
            set { _ErrorMessage = value; }
        }

        public bool InErrorState
        {
            get { return string.IsNullOrEmpty(_ErrorMessage); }
        }

        private string _QualifiedFilename;

        public XMLFileSignature(string QualifiedFilename)
        {
            if (System.IO.File.Exists(QualifiedFilename) == false)
            {
                _ErrorMessage = "'" + QualifiedFilename + "' does not exist for signature retrieval.";
                return;
            }
            _QualifiedFilename = QualifiedFilename;
            MillimanCommon.FileSignature FS = new FileSignature();
            string Signature = FS.GetSignature(QualifiedFilename);
            if (string.IsNullOrEmpty(Signature) == true)
            {
                _ErrorMessage = "'" + QualifiedFilename + "' has not been signed.";
                return;
            }

            if ( string.IsNullOrEmpty(_ErrorMessage ) )
               SignatureToDictionary(Signature);
            
            
        }

        private void SignatureToDictionary(string Signature)
        {
            Polenter.Serialization.SharpSerializer serializer = new Polenter.Serialization.SharpSerializer();
            System.IO.MemoryStream stream = new System.IO.MemoryStream(Utilities.StringToByteArray(Signature));

            _SignatureDictionary = serializer.Deserialize(stream) as Dictionary<string, string>;
        }

        private string DictionaryToString()
        {
            Polenter.Serialization.SharpSerializer serializer = new Polenter.Serialization.SharpSerializer();
            string XML = "";
            using (var stream = new System.IO.MemoryStream())
            {
                // serialize
                serializer.Serialize(_SignatureDictionary, stream);
                // reset stream
                stream.Position = 0;
                XML = Encoding.ASCII.GetString(stream.ToArray()); 
            }
            return XML.Substring(XML.IndexOf("<"));  //trim off all the prefix ??? junk
        }

        public bool SaveChanges()
        {
            try
            {
                if (System.IO.File.Exists(_QualifiedFilename))
                {
                    string XML = DictionaryToString();
                    if (string.IsNullOrEmpty(XML) == false)
                    {
                        FileSignature FS = new FileSignature();
                        FS.SignFile(_QualifiedFilename, XML);
                    }
                    return true;
                }
            }
            catch (Exception)
            {
            }
            return false;
        }
    }
}
