<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="MillimanDev2.Profile" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>User Profile/Password Settings</title>
    <style type="text/css">
    html
   {
    overflow    : hidden;
   }
    body
     {
		margin: 20px 20px 20px 20px;
	}

        .auto-style1 {
            width: 410px;
        }

        button:hover {
            background-color:#BDCAB4;
        }
    </style>

     <script type="text/javascript">

         var GlobalError = '';
         var PasswordFormatError = "Passwords must have a length of 7 characters, with at least 1 non-alphanumeric character.";

         function getRadWindow() {
             var oWindow = null;
             if (window.radWindow)
                 oWindow = window.radWindow;
             else if (window.frameElement.radWindow)
                 oWindow = window.frameElement.radWindow;
             return oWindow;
         }

         // Reload parent page
         function CloseDialog() {
             var ThisDialog = getRadWindow();
             var Parent = getRadWindow().BrowserWindow;
             // Parent.alert("Profile/Password information has been saved.");
             Parent.radalert('Profile/Password information has been saved.', 350, 150, "Information Saved" );
             ThisDialog.close();
         }

   </script>
</head>
<body style="background-color:white;background-image:url(images/watermark.png);background-repeat:repeat" onload="OnLoad();">
    <form id="form1" runat="server">
    <table style="border:0px solid gray;width:850px;margin:0px auto;" cellspacing="5" id="PrimaryContainer">
        <tr>
            <td colspan="2" style=" padding-right:5px;">
                <div id="NewUserMessage" style="background-color:white;width:100%;display:block;border:1px solid #316563">
                    <table border="0" style="padding:5px; overflow: hidden; font-weight:300 ;width:100%" cellpadding="5" >
                        <tr>
                            <td>
                                <img src="css/milliman_small.png" style="border:0px" />
                            </td>
                            <td valign="middle">
                                You are required to change your password and complete your profile information before continuing.&nbsp; Passwords must have a minimum length of 7 characters with at least 1 non-alphanumeric character.
                            </td>
                        </tr>
                    </table>
                </div>
            </td> 
       </tr>

        <tr>
            <td class="auto-style1">
                 <div style="width:400px;height:400px;">
                    <table  cellpadding="5" style="border:1px solid black;width:400px;flex-align:center">
                        <tr>
                            <td  colspan="2" style=" text-align:center; background-image:url(images/header.gif);border-bottom:1px solid black;">User Profile Information</td>
                        </tr>
                        <tr>
                            <td align="right">
                                <label>First Name</label>
                            </td>
                            <td>
                                <asp:TextBox ID="UserFirstName" runat="server"></asp:TextBox>
                                </td>
                        </tr>

                        <tr>
                            <td align="right">
                                <label>Last Name</label>
                            </td>
                            <td>
                                <asp:TextBox ID="UserLastName" runat="server"></asp:TextBox>
                            </td>
                        </tr>

                       <tr>
                            <td align="right">
                                <label>Email</label>
                            </td>
                            <td>
                                <asp:TextBox ID="Email" runat="server"></asp:TextBox><img src="Images/star-blue-icon.png" title="This field is required."/>
                            </td>
                        </tr>

                     <tr>
                            <td align="right">
                                <label>Company</label>
                            </td>
                            <td>
                                <asp:TextBox ID="Company" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                     <tr>
                            <td align="right">
                                <label>Address 1</label>
                            </td>
                            <td>
                                <asp:TextBox ID="Address1" runat="server"></asp:TextBox>
                            </td>
                        </tr>

                     <tr>
                            <td align="right">
                                <label>Address 2</label>
                            </td>
                            <td>
                                <asp:TextBox ID="Address2" runat="server"></asp:TextBox>
                            </td>
                        </tr>

                     <tr>
                            <td align="right">
                                <label>City</label>
                            </td>
                            <td>
                                <asp:TextBox ID="City" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                     <tr>
                            <td align="right">
                                <label>State</label>
                            </td>
                            <td>
                                <asp:DropDownList id="State" runat="server">
	                                <asp:ListItem value="AL">Alabama</asp:ListItem>
	                                <asp:ListItem value="AK">Alaska</asp:ListItem>
	                                <asp:ListItem value="AZ">Arizona</asp:ListItem>
	                                <asp:ListItem value="AR">Arkansas</asp:ListItem>
	                                <asp:ListItem value="CA">California</asp:ListItem>
	                                <asp:ListItem value="CO">Colorado</asp:ListItem>
	                                <asp:ListItem value="CT">Connecticut</asp:ListItem>
	                                <asp:ListItem value="DE">Delaware</asp:ListItem>
	                                <asp:ListItem value="DC">District of Columbia</asp:ListItem>
	                                <asp:ListItem value="FL">Florida</asp:ListItem>
	                                <asp:ListItem value="GA">Georgia</asp:ListItem>
	                                <asp:ListItem value="HI">Hawaii</asp:ListItem>
	                                <asp:ListItem value="ID">Idaho</asp:ListItem>
	                                <asp:ListItem value="IL">Illinois</asp:ListItem>
	                                <asp:ListItem value="IN">Indiana</asp:ListItem>
	                                <asp:ListItem value="IA">Iowa</asp:ListItem>
	                                <asp:ListItem value="KS">Kansas</asp:ListItem>
	                                <asp:ListItem value="KY">Kentucky</asp:ListItem>
	                                <asp:ListItem value="LA">Louisiana</asp:ListItem>
	                                <asp:ListItem value="ME">Maine</asp:ListItem>
	                                <asp:ListItem value="MD">Maryland</asp:ListItem>
	                                <asp:ListItem value="MA">Massachusetts</asp:ListItem>
	                                <asp:ListItem value="MI">Michigan</asp:ListItem>
	                                <asp:ListItem value="MN">Minnesota</asp:ListItem>
	                                <asp:ListItem value="MS">Mississippi</asp:ListItem>
	                                <asp:ListItem value="MO">Missouri</asp:ListItem>
	                                <asp:ListItem value="MT">Montana</asp:ListItem>
	                                <asp:ListItem value="NE">Nebraska</asp:ListItem>
	                                <asp:ListItem value="NV">Nevada</asp:ListItem>
	                                <asp:ListItem value="NH">New Hampshire</asp:ListItem>
	                                <asp:ListItem value="NJ">New Jersey</asp:ListItem>
	                                <asp:ListItem value="NM">New Mexico</asp:ListItem>
	                                <asp:ListItem value="NY">New York</asp:ListItem>
	                                <asp:ListItem value="NC">North Carolina</asp:ListItem>
	                                <asp:ListItem value="ND">North Dakota</asp:ListItem>
	                                <asp:ListItem value="OH">Ohio</asp:ListItem>
	                                <asp:ListItem value="OK">Oklahoma</asp:ListItem>
	                                <asp:ListItem value="OR">Oregon</asp:ListItem>
	                                <asp:ListItem value="PA">Pennsylvania</asp:ListItem>
	                                <asp:ListItem value="RI">Rhode Island</asp:ListItem>
	                                <asp:ListItem value="SC">South Carolina</asp:ListItem>
	                                <asp:ListItem value="SD">South Dakota</asp:ListItem>
	                                <asp:ListItem value="TN">Tennessee</asp:ListItem>
	                                <asp:ListItem value="TX">Texas</asp:ListItem>
	                                <asp:ListItem value="UT">Utah</asp:ListItem>
	                                <asp:ListItem value="VT">Vermont</asp:ListItem>
	                                <asp:ListItem value="VA">Virginia</asp:ListItem>
	                                <asp:ListItem value="WA">Washington</asp:ListItem>
	                                <asp:ListItem value="WV">West Virginia</asp:ListItem>
	                                <asp:ListItem value="WI">Wisconsin</asp:ListItem>
	                                <asp:ListItem value="WY">Wyoming</asp:ListItem>
                                </asp:DropDownList>

                            </td>
                        </tr>

                     <tr>
                            <td align="right">
                                <label>Zip Code</label>
                            </td>
                            <td>
                                <asp:TextBox ID="ZipCode" runat="server"></asp:TextBox>
                            </td>
                        </tr>

                       <tr>
                            <td align="right">
                                <label>Phone</label>
                            </td>
                            <td>
                                <asp:TextBox ID="Phone" runat="server"></asp:TextBox><img src="Images/star-blue-icon.png" title="A phone number is required - valid formats:<br>(999)999-9999 , 999-999-9999 or 9999999999"/>
                            </td>
                        </tr>
<%--                       <tr>
                            <td align="right">
                                <label>Mobile</label>
                            </td>
                            <td>
                                <asp:TextBox ID="Mobile" runat="server"></asp:TextBox><img src="Images/star-blue-icon.png" title="A 'phone' or 'mobile' number is required."/>
                            </td>
                        </tr>--%>

                       <%-- <tr >
                            <td colspan="2" align="center"> 
                                
                                &nbsp;</td> 
                        </tr>--%>
                        </table>
                 </div>
            </td>

            <td style="vertical-align:top">
                <table>
                      <tr>
                        <td >
                             <div style="width:400px; ">

                                <table  cellpadding="5" style="border:1px solid black;width:400px;flex-align:center">
                                        <tr>
                                            <td  colspan="3" style=" text-align:center; background-image:url(images/header.gif);border-bottom:1px solid black;">Password Settings</td>
                                        </tr>                                            
                                         <tr>
                                                <td align="right" style="width:170px">
                                                    <asp:Label ID="CurrentPasswordLabel" runat="server" AssociatedControlID="CurrentPassword">Current Password</asp:Label>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="CurrentPassword" runat="server" TextMode="Password" Width="170px" onblur="LostFocus()"></asp:TextBox>
                                                </td>
                                                <td><img src="Images/star-blue-icon.png" title="This field is required." id="CurrentPasswordIcon" /></td>
                                            </tr>
                                            <tr>
                                                <td align="right" style="width:170px">
                                                    <asp:Label ID="NewPasswordLabel" runat="server" AssociatedControlID="NewPassword">New Password</asp:Label>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="NewPassword" runat="server" onblur="LostFocus()" TextMode="Password" Width="170px" ToolTip="Passwords must have a length of 7 characters, with at least 1 non-alphanumeric character."></asp:TextBox>
                                                </td>
                                                <td><img src="Images/star-blue-icon.png" title="This field is required." id="NewPasswordIcon"/></td>
                                            </tr>
                                            <tr>
                                                <td align="right" style="width:170px">
                                                    <asp:Label ID="ConfirmNewPasswordLabel" runat="server" AssociatedControlID="ConfirmNewPassword">Confirm New Password</asp:Label>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="ConfirmNewPassword" runat="server" onblur="LostFocus()" TextMode="Password" Width="170px" ToolTip="Passwords must have a length of 7 characters, with at least 1 non-alphanumeric character."></asp:TextBox>
                                                </td>
                                                <td><img src="Images/star-blue-icon.png" title="This field is required." id="ConfirmPasswordIcon"/></td>
                                            </tr>

                                        </table>
                            </div>
                        </td>
                      </tr>
                    <tr><td colspan="2">&nbsp;</td></tr>
                       <tr>
                           <td >
                                 <div style="width:400px; ">
                                <table  cellpadding="5" style="border:1px solid black;width:400px;flex-align:center">
                                        <tr>
                                            <td  colspan="2" style=" text-align:center; background-image:url(images/header.gif);border-bottom:1px solid black;">Password Recovery Settings</td>
                                        </tr>
                                        <tr>
                                        <td align="right">
                                            <label>Security Question</label>
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="SecretPhraseDropdown" runat="server">
                                                <asp:ListItem Text="">What was the name of the city you grew up in?</asp:ListItem>
                                                <asp:ListItem Text="">What was your first pet's name?</asp:ListItem>
                                                <asp:ListItem Text="">What model was your first car?</asp:ListItem>
                                                <asp:ListItem Text="">What is your favorite city?</asp:ListItem>
                                                <asp:ListItem Text="">Who manufactured your television?</asp:ListItem>
                                                <asp:ListItem Text="">What is your favorite animal?</asp:ListItem>
                                                <asp:ListItem Text="">What is your lucky number?</asp:ListItem>
                                                <asp:ListItem Text="">What was your childhood nickname?</asp:ListItem>
                                                <asp:ListItem Text="">What is your favorite vacation place?</asp:ListItem>

                                            </asp:DropDownList>
                                            
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            <label>Answer</label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="Answer" runat="server"></asp:TextBox><img src="Images/star-blue-icon.png" title="This field is required." />
                                        </td>
                                    </tr>
                                </table>
  
                            </div>
                                <br />
                                <div id="UserMessages" style="border:0px solid #DF3A01;margin:5px;vertical-align:middle;display:none">
                                   <div style="float:left;padding:5px"> <img src="Images/warning-orange.png"  /></div>
                                    <div id="Message" style="float:left;margin:5px;vertical-align:middle;width:300px"> </div>
                                </div>
                           </td>
                        </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td style="height:15px;">&nbsp;</td>
        </tr>
        <tr>
            <td colspan="2" style="flex-align:center;text-align:center"> 
                <asp:Button ID="ChangePasswordPushButton" runat="server" CommandName="ChangePassword" Text="Apply Changes"  OnClick="ChangePasswordPushButton_Click" OnClientClick="return Validate()" BackColor="White" BorderColor="#CCCCCC" BorderStyle="Solid" BorderWidth="1" Height="25px" Width="195px" style=" background-image:url(images/header.gif)" />
<%--                <asp:ImageButton  ImageUrl="~/Images/applychanges.png" ID="Button1" runat="server" CommandName="ChangePassword"  OnClick="ChangePasswordPushButton_Click" OnClientClick="return Validate()"  />--%>
           </td>
        </tr>
    </table>

      <div id="footer"  style="height:25px;bottom:0;position:absolute;left:10px;right:10px;overflow:hidden;vertical-align:bottom;display:none">
            <table style="width:100%;height:100%;overflow:hidden">
                <tr>
                    <td valign="middle"><center>Copyright &copy Milliman 2015</center></td>
                </tr>
            </table> 
        </div>

    </form>

    <script type="text/javascript">

        var GlobalError = '';
        var PasswordFormatError = "Passwords must have a length of 7 characters, with at least 1 non-alphanumeric character.";

      
        function ErrorDialog() {
            alert('There was an issue saving your information.  An email has been automatically sent to the system administrator on this issue.');

        }

        function OnLoad() {
            var newPasswordIcon = document.getElementById("NewPasswordIcon");
            var confirmPasswordIcon = document.getElementById("ConfirmPasswordIcon");
            var currentPasswordIcon = document.getElementById("CurrentPasswordIcon");
            currentPasswordIcon.style.display = 'none';

            if (window.location.href.indexOf('newuser') == -1) {
                var Header = document.getElementById('NewUserMessage');
                Header.style.display = 'none';
                newPasswordIcon.style.display = 'none';
                confirmPasswordIcon.style.display = 'none';
            }
            else {
                newPasswordIcon.style.display = 'block';
                confirmPasswordIcon.style.display = 'block';
                var Primary = document.getElementById('PrimaryContainer');
                Primary.style.borderWidth = '1px;';
                var Footer = document.getElementById('footer');
                Footer.style.display = 'block';
            }
        }

        function Validate() {
            var MsgBox = document.getElementById("UserMessages");
            MsgBox.style.display = 'none';
            var Msg = document.getElementById("Message");
            var MyEmail = document.getElementById("Email");
  
            if (MyEmail.value == '') {
                MsgBox.style.display = 'block';
                Msg.innerHTML = "The 'Email' field is required.";
                MyEmail.focus();
                return false;
            }
            var Phone = document.getElementById("Phone");
            //var Mobile = document.getElementById("Mobile");
            if ( Phone.value == '' ) 
            {
                MsgBox.style.display = 'block';
                Msg.innerHTML = "The 'Phone' field is required.";
                Mobile.focus();
                return false;
            }

            if ( (Phone.value != '') && (IsValidPhoneNumber(Phone.value) == false) ) {
                MsgBox.style.display = 'block';
                Msg.innerHTML = GlobalError;
                Phone.focus();
                return false;
            }
            //if ( (Mobile.value != '') && (IsValidPhoneNumber(Mobile.value) == false) ) {
            //    MsgBox.style.display = 'block';
            //    Msg.innerHTML = GlobalError;
            //    Mobile.focus();
            //    return false;
            //}
            var passwordAnswer = document.getElementById("Answer");
            if (passwordAnswer.value == '') {
                MsgBox.style.display = 'block';
                Msg.innerHTML = "The password retrieval setting - 'Answer' is required.";
                passwordAnswer.focus();
                return false;
            }
            var CurPassword = document.getElementById("CurrentPassword");
            var newPassword = document.getElementById("NewPassword");
            var confirmPassword = document.getElementById("ConfirmNewPassword");

            if (CurPassword.value != '') {
                if (newPassword.value == '') {
                    MsgBox.style.display = 'block';
                    Msg.innerHTML = "The password setting - 'New Password' cannot be empty";
                    newPassword.focus();
                    return false;
                }
                if (newPassword.value != confirmPassword.value) {
                    MsgBox.style.display = 'block';
                    Msg.innerHTML = "The password setting - 'New Password' and 'Confirm New Password' do not match!";
                    newPassword.focus();
                    return false;
                }
            }

            if ( (newPassword.value != '') || (confirmPassword.value != '') )
            {
                if (CurPassword.value == '') {
                    MsgBox.style.display = 'block';
                    Msg.innerHTML = "To change your password, your 'Current Password' must be provided along with the requested 'New Password' and 'Confirm New Password'.";
                    CurPassword.focus();
                    return false;
                }
                else {
                    if (IsValidPassword(newPassword.value) == false) {
                        MsgBox.style.display = 'block';
                        Msg.innerHTML = PasswordFormatError;
                        newPassword.focus();
                        return false;
                    }
                }
            }


            return true;
        }

        function LostFocus() {
            var CPI = document.getElementById("CurrentPasswordIcon");
            var NPI = document.getElementById("NewPasswordIcon");
            var CNPI = document.getElementById("ConfirmPasswordIcon");

            var CP = document.getElementById("CurrentPassword");
            var NP = document.getElementById("NewPassword");
            var CNP = document.getElementById("ConfirmNewPassword");

            if ( (CP.value != '') || (NP.value != '') || ( CNP.value != ''))
            {
                NPI.style.display = 'block';
                CNPI.style.display = 'block';
   
                if ( CP.value.toLowerCase().indexOf("[system provided]") == -1 )
                    CPI.style.display = 'block';
            }
            else {
                CPI.style.display = 'none';
                NPI.style.display = 'none';
                CNPI.style.display = 'none';
            }
        }

        function IsValidPhoneNumber(PhoneNumber) {
            var formats = "(999)999-9999|999-999-9999|9999999999";
            var r = RegExp("^(" +
                           formats
                             .replace(/([\(\)])/g, "\\$1")
                             .replace(/9/g, "\\d") +
                           ")$");
            if (r.test(PhoneNumber) == false) {
                GlobalError = "An invalid phone number format was entered.  Please use one of the following formats:&#10;(999)999-9999&#10;999-999-9999&#10;9999999999";
                return false;
            }
            return true;
        }

        function IsValidPassword(CandidatePassword) {
            if (/[^a-zA-Z0-9]/.test(CandidatePassword)) {
                //alert('Input is not alphanumeric');
                return CandidatePassword.length >= 7;
            }
            return false;
        }
    </script>
</body>
</html>
