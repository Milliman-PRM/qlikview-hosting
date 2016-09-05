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
 
    //account name without optional password
    private string _Account_Name_No_Password;
    public string Account_Name_No_Password
    {
      get { return _Account_Name_No_Password; }
      set { _Account_Name_No_Password = value; }
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

    private string _Password;
    public string Password
    {
        get { return _Password; }
        set { _Password = value; }
    }

    private bool _HasPassword;

    public bool HasPassword
    {
        get { return _HasPassword; }
        set { _HasPassword = value; }
    }
    

	public UserInfo()
	{
        _HasPassword = false;
        SetStatus(StatusType.NONE);
	}

    public UserInfo(string _UserName, bool SendWelcomeEmail, bool _DataAccess)
    {
        _HasPassword = false;
        _Account_Name = _UserName;
        ProcessAccountName(_UserName, out  _Account_Name_No_Password, out _Password);
        _SendWelcomeEmail = SendWelcomeEmail;
        _DataAccess_Required = _DataAccess;
        SetStatus(StatusType.NONE);
    }

    public enum StatusType { NONE, ERROR, SUCCESS, BADPASSWORD };
    public void SetStatus( StatusType theType )
    {
        if (theType == StatusType.NONE)
            _ValidationImage = "~/images/decoy-icon-16px.png";
        else if (theType == StatusType.ERROR)
            _ValidationImage = "~/images/Play-1-Normal-Red-icon.png";
        else if (theType == StatusType.SUCCESS)
            _ValidationImage = "~/images/Play-1-Normal-icon.png";
        else if (theType == StatusType.BADPASSWORD)
            _ValidationImage = "~/images/remove-key-icon.png";
    }

    private bool ProcessAccountName(string AccountName, out string AccountWithoutPassword, out string OptionalPassword)
    {
        AccountWithoutPassword = string.Empty;
        OptionalPassword = string.Empty;

        if ((AccountName.Count(x => x == '[') > 1) || (AccountName.Count(x => x == ']') > 1))
        {
            return false;  //is a multi-email entry, don't try and parse this just return
        }

        string Account = AccountName.Trim();
        if ((Account.Contains('[')) && (Account.Contains(']')))
        {
            HasPassword = true;

            int PasswordLength = Account.IndexOf(']') - Account.IndexOf('[');
            string Password = Account.Substring(Account.IndexOf('[') + 1, PasswordLength-1);

            AccountWithoutPassword = Account.Substring(0, Account.IndexOf('[')).Trim();
            OptionalPassword = Password.Trim();
        }
        else
        {
            AccountWithoutPassword = Account.Trim();
            OptionalPassword = string.Empty;
        }
        return true;
    }

    public bool IsValidPassword()
    {
        if (HasPassword)
        {
            //simple validation
            //min length 7, with 1 non-alpha
            if (Password.Length < 7)
            {
                _ErrorMsg = "Password is less than minimal length of 7 characters required.";
                SetStatus(StatusType.BADPASSWORD);
                return false;
            }
            System.Text.RegularExpressions.Match Valid = System.Text.RegularExpressions.Regex.Match(Password, "[^a-zA-Z]");
            if (Valid.Success == false)
            {
                _ErrorMsg = "Password must contain at least one non-alpha character from the set '0123456789~!@#$%^&*()_-+={[}];:<,>.?'";
                SetStatus(StatusType.BADPASSWORD);
                return false;
            }
        }
        return true;
    }
}