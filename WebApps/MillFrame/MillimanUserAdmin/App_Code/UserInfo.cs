using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for UserInfo
/// </summary>
[Serializable()]
public class UserInfo
{
    private string _Account_Name;

    public string Account_Name
    {
        get { return _Account_Name; }
        set { _Account_Name = value; }
    }
 
    private bool _DataAccess_Required;

    public bool DataAccess_Required
    {
        get { return _DataAccess_Required; }
        set { _DataAccess_Required = value; }
    }

    private string _ValidationImage;

    public string ValidationImage
    {
        get { return _ValidationImage; }
        set { _ValidationImage = value; }
    }

    private bool _SendWelcomeEmail = true;

    public bool SendWelcomeEmail
    {
        get { return _SendWelcomeEmail; }
        set { _SendWelcomeEmail = value; }
    }

    private string _ErrorMsg;

    public string ErrorMsg
    {
        get { return _ErrorMsg; }
        set { _ErrorMsg = value;
                if (string.IsNullOrEmpty(_ErrorMsg) == false)
                SetStatus(StatusType.ERROR);
            }
    }


	public UserInfo()
	{
        SetStatus(StatusType.NONE);
	}

    public UserInfo(string _AccountName, bool SendWelcomeEmail, bool _DataAccess)
    {
        _Account_Name = _AccountName;
        _SendWelcomeEmail = SendWelcomeEmail;
        _DataAccess_Required = _DataAccess;
        SetStatus(StatusType.NONE);
    }

    public enum StatusType { NONE, ERROR, SUCCESS };
    public void SetStatus( StatusType theType )
    {
        if ( theType == StatusType.NONE )
            _ValidationImage = "~/images/decoy-icon-16px.png";
        else if ( theType == StatusType.ERROR )
            _ValidationImage = "~/images/Play-1-Normal-Red-icon.png";
        else if (theType == StatusType.SUCCESS)
            _ValidationImage = "~/images/Play-1-Normal-icon.png";
    }
}