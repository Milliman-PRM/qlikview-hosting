using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.Profile;

namespace MillimanDev2
{
    public partial class Profile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
                if (Request["newuser"] != null)
                {
                    CurrentPassword.Enabled = false;
                    CurrentPassword.BackColor = System.Drawing.Color.LightGray;
                    CurrentPassword.TextMode = TextBoxMode.SingleLine;
                    CurrentPassword.Text = @"[System Provided]";
                }
            }
        }

        private string GetValueAsString(ProfileBase PB, string ID)
        {
            object Value = PB.GetPropertyValue(ID);
            return Value == null ? "" : Value.ToString();
        }
        private bool SetValueFromString(ProfileBase PB, string ID, string Value )
        {
            try
            {
                PB.SetPropertyValue(ID, Value);
                return true;
            }
            catch (Exception)
            {

            }
            return false;
        }

        private void LoadData()
        {
            ProfileBase PB = ProfileBase.Create(Membership.GetUser().UserName);
            UserFirstName.Text = GetValueAsString( PB, "Personal.FirstName");
            UserLastName.Text = GetValueAsString( PB, "Personal.LastName");
            Email.Text = Membership.GetUser().Email;
            Phone.Text = GetValueAsString( PB, "Company.Phone");

            if (string.IsNullOrEmpty( Membership.GetUser().PasswordQuestion ) == false )
                SecretPhraseDropdown.Text = Membership.GetUser().PasswordQuestion;
            Answer.Text = string.IsNullOrEmpty(Membership.GetUser().Comment) ? "" : Membership.GetUser().Comment;  //we put the answer in the comment
        }

        private void SaveData()
        {
            ProfileBase PB = ProfileBase.Create(Membership.GetUser().UserName);
 
            SetValueFromString(PB, "Personal.FirstName", SentenceCase(UserFirstName.Text));
            SetValueFromString(PB, "Personal.LastName", SentenceCase(UserLastName.Text));
            SetValueFromString(PB, "Company.Phone", Phone.Text);
            PB.Save();

            MembershipUser MU = Membership.GetUser();
            MU.Email = Email.Text;
            MU.Comment = Answer.Text;
            Membership.UpdateUser(MU);

            if (Request["newuser"] != null)
            {
                MU.ChangePassword(MU.GetPassword(), NewPassword.Text);
            }
           // string password = Membership.Providers["dbSqlMemberShipProviderAdmin"].GetPassword(MU.UserName, null);
            string password = MU.GetPassword();
            MU.ChangePasswordQuestionAndAnswer(password, SecretPhraseDropdown.Text, Answer.Text);           

        }

        /// <summary>
        /// Converts a string to sentence case.
        /// </summary>
        /// <param name="input">The string to convert.</param>
        /// <returns>A string</returns>
        public static string SentenceCase(string input)
        {
            if (input.Length < 1)
                return input;

            string sentence = input.ToLower();
            return sentence[0].ToString().ToUpper() +
               sentence.Substring(1);
        }

        private enum DataValidationTypes { DATA_VALID, BAD_CURRENT_PASSWORD, NO_PASSWORD_CHANGE, EMBEDDED_PASSWORD, UNSPECIFIED_ERROR };
        private DataValidationTypes ValidateData()
        {
            try
            {
                //user isn't changing password settings
                if (string.IsNullOrEmpty(CurrentPassword.Text) && string.IsNullOrEmpty(NewPassword.Text) && string.IsNullOrEmpty(ConfirmNewPassword.Text))
                    return DataValidationTypes.NO_PASSWORD_CHANGE;

                MembershipUser MU = Membership.GetUser();
                //       not empty                                must be the same  New & Conf                           Old and New not same                                          New does nothave old embedded      
                if ((ConfirmNewPassword.Text != "") && (ConfirmNewPassword.Text == NewPassword.Text) && (string.Compare(MU.GetPassword(),NewPassword.Text,true)!=0) && ( NewPassword.Text.ToLower().IndexOf( MU.GetPassword().ToLower() ) == -1 ) )
                {
                    string CurrentSystemPassword = string.Compare(CurrentPassword.Text, "[system provided]", true) == 0 ? Membership.GetUser().GetPassword() : CurrentPassword.Text;
                    if (Membership.GetUser().ChangePassword(CurrentSystemPassword, ConfirmNewPassword.Text) == true) 
                        return DataValidationTypes.DATA_VALID;
                    return DataValidationTypes.BAD_CURRENT_PASSWORD;
                }
                return DataValidationTypes.EMBEDDED_PASSWORD;
            }
            catch (Exception ex)
            {
                MillimanCommon.Report.Log( MillimanCommon.Report.ReportType.Error,"Could not change user password for user " + Membership.GetUser().Email + " Error-" + ex.ToString());
            }
            return DataValidationTypes.UNSPECIFIED_ERROR;

        }

        protected void ChangePasswordPushButton_Click(object sender, EventArgs e)
        {

            try 
	            {
                        DataValidationTypes DVT = ValidateData();
		                if (DVT == DataValidationTypes.NO_PASSWORD_CHANGE || DVT == DataValidationTypes.DATA_VALID )
                        {
                            SaveData();

                            if (Request["newuser"] != null)
                            {
                                string ResetFile = System.IO.Path.Combine(WebConfigurationManager.AppSettings["ResetUserInfoRoot"], Membership.GetUser().ProviderUserKey.ToString() + ".rst");
                                System.IO.File.Delete(ResetFile);
                                Response.Redirect("default.aspx");
                            }
                            else
                            {
                                //close the window
                                Page.ClientScript.RegisterClientScriptBlock(GetType(), "CloseScript", "CloseDialog()", true);
                            }
                        }
                        else if ( DVT == DataValidationTypes.BAD_CURRENT_PASSWORD )
                        {
                            string BadPassword = "Your 'Current Password' could not be confirmed.";
                            MillimanCommon.Alert.Show(BadPassword);
                        }
                        else if (DVT == DataValidationTypes.EMBEDDED_PASSWORD)
                        {
                            string BadPassword =  "Your 'New Password' cannot be the same as the 'Current Password',  or have the 'Current Password' embedded with in it.";
                            MillimanCommon.Alert.Show(BadPassword);
                        }
	            }
	            catch (Exception ex)
	            {
                     MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Change password error", ex );
                     Page.ClientScript.RegisterClientScriptBlock(GetType(), "ErrorScript", "ErrorDialog()", true);
	            }
            
        }
    }
}