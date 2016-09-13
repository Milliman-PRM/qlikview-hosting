using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for PasswordGenerator
/// </summary>
public class PasswordGenerator
{
	public PasswordGenerator()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static string Generate( string PrefixWith = "")
    {
        string NewPassword = Guid.NewGuid().ToString("N").Substring(0, 8);
            NewPassword = NewPassword.Replace('0', 'x');
            NewPassword = NewPassword.Replace('o', 'z');
            NewPassword = NewPassword.Replace('O', 'N');
            NewPassword = NewPassword.Replace('1', '3');
            NewPassword = NewPassword.Replace('l', 'R');
        return PrefixWith + NewPassword;
    }
}