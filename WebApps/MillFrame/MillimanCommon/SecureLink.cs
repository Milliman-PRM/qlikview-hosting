using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MillimanCommon
{
    public class SecureLink
    {

        static public string CreateSecureLink( string AdminName, string UserName, string UserID )
        {
            string CurrentDateTime = System.DateTime.Now.ToString();
            string Signature = "<xml><username>" + UserName + "</username><userid>" + UserID + "</userid><stamp>" + CurrentDateTime + "</stamp><adminname>" + AdminName + "</adminname></xml>";
            AutoCrypt AC = new AutoCrypt();
            return AC.AutoEncrypt(Signature, true);// AC.AutoEncrypt(Signature);
        }

        static public bool IsSecureLinkValid(string Signature, out string AdminName, out string UserName, out string UserID, out DateTime CertCreatedOn)
        {
            AdminName = string.Empty;
            UserName = string.Empty;
            UserID = string.Empty;
            CertCreatedOn = DateTime.MinValue;
            try
            {
                AutoCrypt AC = new AutoCrypt();
                string XML = AC.AutoDecrypt(Signature, true);

                string[] Tokens = XML.Split(new char[] { '<', '>' });
                int Index = 0;
                while (Index < Tokens.Count())
                {
                    if (string.Compare(Tokens[Index], "username", true) == 0)
                    {
                        UserName = Tokens[Index + 1];
                        Index++;
                    }
                    else if (string.Compare(Tokens[Index], "adminname", true) == 0)
                    {
                        AdminName = Tokens[Index + 1];
                        Index++;
                    }
                    else if (string.Compare(Tokens[Index], "userid", true) == 0)
                    {
                        UserID = Tokens[Index + 1];
                        Index++;
                    }
                    else if (string.Compare(Tokens[Index], "stamp", true) == 0)
                    {
                        CertCreatedOn = System.DateTime.Parse(Tokens[Index + 1]);
                        Index++;
                    }
                    Index++;
                }
                string SecureLinkLifeSpan = System.Configuration.ConfigurationManager.AppSettings["SecureLinkLifeSpan"];

                System.TimeSpan TS = System.DateTime.Now - CertCreatedOn;

                if (TS.Days <= System.Convert.ToInt32(SecureLinkLifeSpan))
                    return true;
            }
            catch (Exception ex)
            {
                MillimanCommon.Report.Log(Report.ReportType.Error, "Invalid secure link certificate", ex);
            }
            return false;
        }
    }
}
