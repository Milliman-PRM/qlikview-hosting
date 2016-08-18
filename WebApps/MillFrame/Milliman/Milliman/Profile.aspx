<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="MillimanDev2.Profile" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>User Profile/Password Settings</title>
    <link id="lnkBootstrapcss" runat="server" rel="stylesheet" type="text/css" href="~/Css/bootstrap.css" />
    <style type="text/css">
        html {
            overflow:scroll;
        }
        html,button,input,select,textarea,label{font-family:arial,"Times New Roman",Times,serif,sans-serif;font-size:12px;color:#222}
        body{margin:20px}
        /*Standard Generic*/
        .space{margin-bottom:5px}
        .alert{border:1px solid transparent;border-radius:4px!important;margin-bottom:6px!important;padding:4px!important}
        .weak{color:#999;font-size:14px}
        .labelweak{color:#5f6162;font-size:12px;margin:5px -6px 3px 8px}
        .required:after{content:" *";font-weight:700;color:#c1c1c1;font-size:20px;margin:0;float:right}
        .textbox-focus{border:solid 2px #00C;background:#def}
         input.textbox-focus{border:2px solid #00C;background:#def}
        .roundShadowContainer{background:#fefefe;border:1px solid #c4cddb;padding:0;margin:0 auto;border-top-color:#d3dbde;border-bottom-color:#bfc9dc;box-shadow:0 1px 1px #ccc;border-radius:5px;}
        .roundShadowContainer h2{margin:0;padding:10px 0;font-size:18px;text-align:center;background:#ebebec;border-bottom:1px solid #dde0e7;box-shadow:0 -1px 0 #fff inset;border-radius:5px 5px 0 0;text-shadow:1px 1px 0 #fff;font-weight:700}
        .roundShadowContainer h3{margin:0;padding:10px 0;font-size:16px;text-align:center;background:#ebebec;border-bottom:1px solid #dde0e7;box-shadow:0 -1px 0 #fff inset;border-radius:5px 5px 0 0;text-shadow:1px 1px 0 #fff;font-weight:700}
        .roundShadowContainer ul,li{margin:0;padding:0;list-style-type:square}
        .roundShadowContainer input{border:1px solid #d5d9da;border-radius:5px;box-shadow:0 0 5px #e8e9eb inset;outline:0}
        .roundShadowContainer {width:435px;}
        .fieldSetWithBorder{border:2px dashed #eee;margin:2px 2px 3px 3px;overflow:hidden;padding:2px;width:217px}
        .fieldSetWithBorder legend{font-size:14px;line-height:inherit}
         .first-of-type{margin:4px 3px 4px -2px}
        .addBar{font-size:14px;font-weight:700;margin:-1px 36px -3px;overflow:hidden;width:300px}
        .ddl{border:2px solid #d5d9da;border-radius:3px;padding:3px;text-indent:.01px;font-size:13px;font-family:Georgia;margin:6px 6px 0 2px}
         /*Password Generic*/
        #divPasswordCriteriaContainer{font-size:.8em;}
        #divPasswordCriteriaContainer{display:none;padding:6px;position:absolute;width:236px;z-index:2000;}
        #divPasswordCriteriaContainer:before{content:"\25B2";position:absolute;top:-10px;left:45%;font-size:14px;line-height:12px;color:#ddd;display:block}
        #divPasswordCriteriaContainer h1,h2,h3,h4{margin:0 0 10px;padding:0;font-weight:400}
        #divPasswordCriteriaContainer ul,li{list-style-type:circle;margin:4px 3px 4px 4px;padding:2px 1px 3px 9px;width:203px}
        .invalidPassword,.validPassword{padding-left:6px;line-height:12px}
        .invalidPassword{color:#ec3f41}
        .validPassword{color:#3a7d34}
        .badMatch{color:#f66}
        .dashedSeparator{margin:25px 0;border-bottom:dashed 1px #666}
        /*bootstra specifc*/
        .container{width:832px}
         td{padding:4px!important}
        .form-control{height:30px}
        .table{margin-bottom:5px}
        /*password hint*/
        .PWHsmallContainer{background: #f4f9fb none repeat scroll 0 0;border: 2px dashed #ddd;width: 349px;}     
        .PWHshowHideDivHeader{width: 123px;padding:0;margin:4px 8px 9px 1px;cursor:pointer;}
        .showPassword{float:right;margin:-25px 13px 0 0}          
        .listUL{font-size:12px;list-style-type:square;margin:2px 0 1px 12px;padding:2px 1px 1px 6px;width:312px}
        .listLi{font-size:12px;list-style-type:square;margin:3px 0 -2px 32px;padding:2px 1px 1px 6px;width:267px}   
        #divPasswordHintCriteria{background:#fefefe none repeat scroll 0 0;border:1px solid #ddd;
                                 border-radius:5px;box-shadow:0 1px 3px #ccc;display:none;font-size:.8em;
                                 height:165px;padding:6px;position:absolute;width:365px;z-index:2000}
  
.containerWrap
{
  text-align: center;
  padding: 15px;  
  width: 100%;
}
.left-div
{
  display: inline-block;
  max-width: 435px;
  text-align: left;
  padding: 3px;
  margin: 3px;
  vertical-align: top;
}
.right-div
{
  display: inline-block;
  max-width: 435px;
  text-align: left;
  padding: 3px;
  margin: 3px;
}

    </style>

    <script type="text/javascript">
        var GlobalError = '';
        var PasswordFormatError = "Passwords must have a length of 8 characters, with at least 1 non-alphanumeric character.";

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
            Parent.radalert('Profile/Password information has been saved.', 350, 150, "Information Saved");
            ThisDialog.close();
        }

        //Clear the Text Boxes
        function ClearTextboxes() {
            try {
                document.getElementById('UserFirstName').value = '';
                document.getElementById('UserLastName').value = '';
                //document.getElementById('Email').value = '';
                document.getElementById('Phone').value = '';
                document.getElementById('CurrentPassword').value = '';
                document.getElementById('NewPassword').value = '';
                document.getElementById('ConfirmNewPassword').value = '';
                document.getElementById('Answer').value = '';

                //select first element in drop down list
                var ddl = $('select[name="SecretPhraseDropdown"]');
                ddl.val(ddl.find('option').first().val());
            }
            catch (err)
            { var txt = 'Errot=>' + err.description; alert(txt); }
        }

    </script>
</head>
<body style="background-color: white; background-image: url(images/watermark.png); background-repeat: repeat;" onload="OnLoad();">
    <form id="form1" runat="server">
        <div class="containerWrap">
            <div class="page-header roundShadowContainer" style="width:880px;">
              <h2>User Profile  <small>Password Settings</small></h2>
            </div>
            <div class="left-div">
                <%--window for the user profile info--%>
                <div id="divUserProfileSettingsContainer" class="roundShadowContainer">
                    <h3>User Profile Information</h3>
                    <table class="table table-hover">
                        <tbody>
                            <tr>
                                <td>
                                    <label for="UserFirstName" class="labelweak">First Name:</label></td>
                                <td>
                                    <input id="UserFirstName" name="UserFirstName" type="text" runat="server" class="form-control" placeholder="first name..."    maxlength="50" style="width: 185px;" tabindex="1" onclick="this.select();" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <label for="UserLastName" class="labelweak">Last Name:</label></td>
                                <td>
                                    <input id="UserLastName" name="UserLastName" type="text" runat="server" class="form-control" placeholder="last name..."
                                        maxlength="50" style="width: 185px;" tabindex="2" onclick="this.select();" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <label for="Email" class="labelweak required">Email Address:&nbsp;</label></td>
                                <td>
                                    <input id="Email" name="Email" type="text" runat="server" class="form-control" placeholder="email" disabled="disabled" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <label for="Phone" class="labelweak required">Phone:&nbsp;</label>
                                </td>
                                <td>
                                    <input id="Phone" name="Phone" type="text" runat="server" class="form-control" placeholder="(000)000-0000"
                                         style="width: 185px;" tabindex="3" />
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                <%--window for the user profile info--%>
                <%--password hint--%>
                <div id="divPasswordHint" class="PWHshowHideDivHeader">
                    <span class="weak">Password Hint&nbsp;<img id="imgShow" src="~/Images/InformationBulb.png" runat="server"
                        width="18" height="18" style="margin: 2px 6px 6px 2px;" />
                    </span>
                    <div id="divPasswordHintCriteria">
                        <div class="PWHsmallContainer">
                            <fieldset class="first-of-type">
                                <legend id="litLegend" class="addBar">Your password must:</legend>
                                <ul class="listUL">
                                    <li class="listLi">Be at least 8 characters long</li>
                                    <li class="listLi">Conatin a capital letter [A-Z]</li>
                                    <li class="listLi">Conatin a lowercase letter [a-z]</li>
                                    <li class="listLi">Conatin a number [0-9]</li>
                                    <li class="listLi">Conatin a special character [~!@#$%^&*;?+_.]</li>
                                </ul>
                            </fieldset>
                        </div>
                    </div>
                </div>
                <%--password hint--%>
            </div>
            <div class="right-div">
                <%--window for the user Password Verification info--%>
                <div class="roundShadowContainer">
                    <h3>Password Verification</h3>
                    <table class="table table-hover">
                        <tbody>
                            <tr>
                                <td>
                                    <label for="CurrentPassword" class="labelweak required">Current Password:&nbsp;</label></td>
                                <td>
                                    <asp:TextBox ID="CurrentPassword" runat="server" class="form-control" TextMode="Password" Width="185px">
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <label for="NewPassword" class="labelweak required">New Password:&nbsp;</label></td>
                                <td>
                                    <input id="NewPassword" name="NewPassword" type="password" runat="server" class="NewPassword form-control"
                                        style="width: 185px;" tabindex="5" onclick="this.select();" />

                                    <img src="Images/eye-icon.png" class="showPassword"
                                        id="imageShowPassword" onclick="changeImage(this)" width="18" height="18" />
                                    <div id="divPasswordCriteriaContainer" class="passwordCriteria roundShadowContainer">
                                        <fieldset class="fieldSetWithBorder">
                                            <legend>Password Criteria</legend>
                                            <div id="divcriteria" style="margin: -15px 0 0 1px;">
                                                <ul>                                                   
                                                    <li id="capital" class="invalidPassword">At least <strong>one capital letter</strong></li>
                                                    <li id="lowercase" class="invalidPassword">At least <strong>one lowercase letter</strong></li>
                                                    <li id="number" class="invalidPassword">At least <strong>one number</strong></li>
                                                    <li id="special" class="invalidPassword">Must contain a <strong>special character</strong></li>
                                                    <li id="length" class="invalidPassword">Be at least <strong>8 characters</strong></li>
                                                </ul>
                                            </div>
                                        </fieldset>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <label for="ConfirmNewPassword" class="labelweak required">Confirm Password:&nbsp;</label></td>
                                <td>
                                    <input id="ConfirmNewPassword" name="ConfirmNewPassword" type="password" runat="server" class="form-control"
                                        style="width: 185px;" tabindex="6" onclick="this.select();" />

                                    <span id="passwordMatchMessage" class="passwordMatchMessage"></span>
                                </td>
                            </tr>
                        </tbody>
                    </table>

                </div>
                <%--window for the user Password Verification info--%>
                <div class="space"></div>
                <%--window for the user Password Recovery Settings--%>
                <div class="roundShadowContainer">
                    <h3>Password Recovery Settings</h3>
                    <table class="table table-hover">
                        <tbody>
                            <tr>
                                <td>
                                    <label for="SecretPhraseDropdown" class="labelweak">Security Question:</label></td>
                                <td>
                                    <asp:DropDownList ID="SecretPhraseDropdown" runat="server"
                                        BackColor="Window" Font-Names="Georgia" CssClass="ddl">
                                        <asp:ListItem Text="">What was the name of the city you grew up in?</asp:ListItem>
                                        <asp:ListItem Text="">What was your first pet's name?</asp:ListItem>
                                        <asp:ListItem Text="">What model was your first car?</asp:ListItem>
                                        <asp:ListItem Text="">What is your favorite city?</asp:ListItem>
                                        <asp:ListItem Text="">Who manufactured your television?</asp:ListItem>
                                        <asp:ListItem Text="">What is your favorite animal?</asp:ListItem>
                                        <asp:ListItem Text="">What is your lucky number?</asp:ListItem>
                                        <asp:ListItem Text="">What was your childhood nickname?</asp:ListItem>
                                        <asp:ListItem Text="">What is your favorite vacation place?</asp:ListItem>
                                    </asp:DropDownList></td>
                            </tr>
                            <tr>
                                <td>
                                    <label for="Answer" class="labelweak required">Answer:&nbsp;</label></td>
                                <td>
                                    <input id="Answer" name="Answer" type="text" runat="server" class="form-control"
                                        style="width: 295px;" tabindex="7" maxlength="128"  onclick="this.select();" />
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                <%--window for the user Password Recovery Settings--%>
            </div>
        </div>
        <div class="space"></div>
        <div class="row">
            <div class="center-block" style="float: none;width: 415px;">
                <div class="alert alert-warning">
                    <strong>Important!</strong> All required fields are marked with an asterisk (*).
                </div>
                <div class="col-md-8" style="float: none;">
                    <asp:Button ID="Button1" runat="server" CommandName="ChangePassword" Text="Apply Changes"
                        OnClick="ChangePasswordPushButton_Click" CssClass="btn btn-primary"
                        OnClientClick="return Validate();" />
                    <asp:Button ID="Button2" runat="server" CommandName="ResetForm" Text="Reset Form" CssClass="btn btn-primary"
                        OnClientClick="ClearTextboxes();" />
                </div>
            </div>
        </div>
        <div id="footer" style="height: 25px; bottom: 0; position: absolute; left: 10px; right: 10px; overflow: hidden; vertical-align: bottom; display: none">
            <table style="width: 100%; height: 100%; overflow: hidden">
                <tr>
                    <td valign="middle">
                        <center>Copyright &copy Milliman 2015</center>
                    </td>
                </tr>
            </table>
        </div>

    </form>

    <script src="https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.4/js/bootstrap.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap3-dialog/1.34.5/js/bootstrap-dialog.min.js"></script>
    <link rel="stylesheet" type="text/css" href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap3-dialog/1.34.5/css/bootstrap-dialog.min.css" />
    <script src="Javascript/maskedinput.js" type="text/javascript"></script>
    <script type="text/javascript">
        var PasswordFormatError = "Passwords must have a length of 8 characters, with at least 1 non-alphanumeric character.";

        //form load
        function OnLoad() {
            var msg = 'You are required to change your password and complete your profile information before continuing.&nbsp <br>Passwords must have a minimum length of 8 characters with at least 1 non-alphanumeric character.';
            if (window.location.href.indexOf('newuser') == -1) {
                //do nothing             
            }
            else {
                //$('#CurrentPassword').at
                showInformationAlert(msg);
            }
        }

        //set phone mask
        $("#Phone").mask("(999) 999-9999");

        // Code to display the password hint next to the image
        var moveLeft = 10;
        var moveDown = 20;
        $('.PWHshowHideDivHeader').hover(function (e) {
            $('#divPasswordHintCriteria').show();
        }, function () {
            $('#divPasswordHintCriteria').hide();
        });

        $('.PWHshowHideDivHeader').mousemove(function (e) {
            $("#divPasswordHintCriteria")
                .css('top', e.pageY + moveDown)
                .css('left', e.pageX + moveLeft);
        });

        //code to show the password criteria next to input box
        var newPassword = document.querySelector('#NewPassword');
        newPassword.onmouseup = function (e) {
            var div = document.querySelector('#divPasswordCriteriaContainer');
            var inp = document.querySelector('#NewPassword');
            var rect = inp.getClientRects();
            div.style.display = 'block';
            div.style.left = rect[0].left + 'px';
            div.style.top = rect[0].bottom + 'px';
        }
        newPassword.onblur = function (e) {
            var div = document.querySelector('#divPasswordCriteriaContainer');
            div.style.display = 'none';
        }

        //function to switch between show hide image
        $("#imageShowPassword").on('click', function () {
            //show password for the class of NewPassword
            var $pwd = $(".NewPassword");
            var imageElement = $("#imageShowPassword");
            if ($pwd.attr('type') === 'password') {
                $pwd.attr('type', 'text');
                changeImage('imageElement');
            } else {
                $pwd.attr('type', 'password');
                changeImage('imageElement');
            }
        });

        function changeImage(element) {
            var right = "./Images/eye-icon.png";
            var left = "./Images/eye-hide.png";
            element.src = element.bln ? right : left;
            element.bln = !element.bln;
        }

        //Prevent cut, copy, paste
        //$('#ConfirmNewPassword').bind("cut copy paste", function (e) {
        //    e.preventDefault();
        //});

        $("#ConfirmNewPassword").keyup(validatePasswordMatch);

        //function to validate data
        function Validate() {
            try {
                //allow only alphabets
                var regixCharsOnly = new RegExp(/^[a-zA-Z]*$/);
                if ($('#UserFirstName').val() != '') {
                    if (!$('#UserFirstName').val().match(regixCharsOnly)) {
                        showErrorAlert('The First name can be characters only.');
                        //$("#UserFirstName").focus();
                        return false;
                    }
                }
                if ($('#UserLastName').val() != '') {
                    if (!$('#UserLastName').val().match(regixCharsOnly)) {
                        showErrorAlert('The last name can be characters only.');
                        //$("#UserLastName").focus();
                        return false;
                    }
                }

                var MyEmail = $('#Email').val();
                if (MyEmail == '') {
                    showErrorAlert('The Email field is required.');
                    //$("#Email").focus();
                    return false;
                }

                var Phone = $('#Phone').val();
                if (Phone == '') {
                    showErrorAlert('The Phone field is required.');
                    //$('#Phone').select();
                    return false;
                }

                var CurPassword = $('#CurrentPassword').val(); //document.getElementById("CurrentPassword").value;
                var newPassword = $('#NewPassword').val(); //document.getElementById("NewPassword").value;
                var confirmPassword = $('#ConfirmNewPassword').val(); //document.getElementById("ConfirmNewPassword").value;

                //check the password only if CurPassword is entered
                if (CurPassword != '') {
                    if (newPassword == '') {
                        var msg = ('The password setting New Password filed cannot be empty.');
                        showErrorAlert(msg);
                        //$("#NewPassword").focus()
                        return false;
                    }

                    if (newPassword.length < 8) {
                        var msg = ('The New Password field length must be at least 8 chars.');
                        showErrorAlert(msg);
                       // $("#NewPassword").focus()
                        return false;
                    }

                    if (confirmPassword == '') {
                        var msg = ('The password setting Confirm New Password filed cannot be empty.');
                        showErrorAlert(msg);
                        //$('#ConfirmNewPassword').focus()
                        return false;
                    }

                    if (confirmPassword.length < 8) {
                        var msg = ('The Confirm Password length must be at least 8 chars.');
                        showErrorAlert(msg);
                       // $('#ConfirmNewPassword').focus()
                        return false;
                    }
                    if (newPassword != confirmPassword) {
                        var msg = ('The password setting New Password filed and Confirm New Password field do not match!');
                        showErrorAlert(msg);
                        $("#NewPassword").focus()
                        $("#NewPassword").select()
                        return false;
                    }
                }
                if ((newPassword != '') || (confirmPassword != '')) {
                    debugger;
                    if (CurPassword == '') {
                        var msg = ('To change your password, your Current Password must be provided along with the requested New Password and Confirm New Password.');
                        showErrorAlert(msg);
                        //CurrentPassword.focus()
                        return false;
                    }
                }

                var passwordAnswer = $('#Answer').val(); //document.getElementById("Answer").v;
                if (passwordAnswer == '') {
                    var msg = ('The password retrieval setting Answer field is required.');
                    showErrorAlert(msg);
                    //$('#Answer').focus()
                    return false;
                }
            }
            catch (err) {
                return false;
                var txt = 'Errot=>' + err.description;
                alert(txt);
            }
            return true;
        }

        //Password Check
        $('input.NewPassword').keyup(function () {
            // set password variable
            var newpasswordValue = $(this).val();

            //validate the length
            if (newpasswordValue.length < 8) {
                $('#length').removeClass('validPassword').addClass('invalidPassword');
            } else {
                $('#length').removeClass('invalidPassword').addClass('validPassword');
            }

            //validate any uppercase letter
            if (newpasswordValue.match(/[A-Z]/)) {
                $('#capital').removeClass('invalidPassword').addClass('validPassword');
            } else {
                $('#capital').removeClass('validPassword').addClass('invalidPassword');
            }

            //validate any lower case letter
            if (newpasswordValue.match(/[a-z]/)) {
                $('#lowercase').removeClass('invalidPassword').addClass('validPassword');
            } else {
                $('#lowercase').removeClass('validPassword').addClass('invalidPassword');
            }

            //validate a number
            if (newpasswordValue.match(/[0-9]/)) {
                $('#number').removeClass('invalidPassword').addClass('validPassword');
            } else {
                $('#number').removeClass('validPassword').addClass('invalidPassword');
            }

            //validate allowed special
            if (newpasswordValue.match(/[~!@#$%^&*;?+_.]/)) {
                $('#special').removeClass('invalidPassword').addClass('validPassword');
            } else {
                $('#special').removeClass('validPassword').addClass('invalidPassword');
            }

            //not allowed chars
            var regexChar = new RegExp(/[`,<>;':"/[\]|{}()=-]/);
            if (newpasswordValue.match(regexChar)) {
                showErrorAlert('The character you entered is not valid.');
                return false;
            }
            //not allowed continus repeated chars
            var regex = new RegExp(/([A-Za-z])\1\1\1/);
            if (newpasswordValue.match(regex)) {
                showErrorAlert('You can not have more than 3 continus repeated chars.');
                return false;
            }


            //validate non-printable chars 
            if (newpasswordValue.match(/[^\u0000-\u007F]/)) {
                showErrorAlert('You can not have non-printable chars.');
                return false;
            }

        }).focus(function () {
            $('#divPasswordCriteriaContainer').show();
        }).blur(function () {
            $('#divPasswordCriteriaContainer').hide();
        });

        //function to check if the two password matches
        var message = document.getElementById('passwordMatchMessage');
        function validatePasswordMatch() {
            var newPassword = $("#NewPassword").val();
            var confirmPassword = $("#ConfirmNewPassword").val();
            if (newPassword != confirmPassword) {
                message.innerHTML = "Passwords Do Not Match!"
                $('#passwordMatchMessage').addClass('badMatch');
            }
            else {
                resetPasswordMatch();
            }
        }

        function resetPasswordMatch() {
            message.innerHTML = "";
            $('#passwordMatchMessage').removeClass('badMatch');
        }

        //show error
        function showErrorAlert(alertMessage) {
            BootstrapDialog.show({
                title: 'Error',
                message: alertMessage,
                type: BootstrapDialog.TYPE_DANGER, // <-- Default value is BootstrapDialog.TYPE_PRIMARY
                closable: true, // <-- Default value is false
                draggable: true, // <-- Default value is false
                buttons: [{
                    label: 'OK',
                    hotkey: 13, // Keycode of keyup event of key 'A' is 65.
                    cssClass: 'btn-danger',
                    action: function (dialog) {
                        dialog.close();
                    }
                }],
            });
        }

        //show info
        function showInformationAlert(alertMessage) {
            BootstrapDialog.show({
                title: 'Information',
                message: alertMessage,
                type: BootstrapDialog.TYPE_INFO, // <-- Default value is BootstrapDialog.TYPE_PRIMARY
                closable: true, // <-- Default value is false
                draggable: true, // <-- Default value is false
                buttons: [{
                    label: 'OK',
                    hotkey: 13, // Keycode of keyup event of key 'A' is 65.
                    cssClass: 'btn-info',
                    action: function (dialog) {
                        dialog.close();
                    }
                }],
            });
        }

    </script>
</body>
</html>
