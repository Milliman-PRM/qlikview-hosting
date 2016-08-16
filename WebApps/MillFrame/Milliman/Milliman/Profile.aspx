<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="MillimanDev2.Profile" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>User Profile/Password Settings</title>
    <link id="lnkBootstrapcss" runat="server" rel="stylesheet" type="text/css" href="~/Css/bootstrap.css" />
    <style type="text/css">
        html{overflow-y:scroll!important;overflow-x:scroll!important}
        html,button,input,select,textarea,label{font-family:arial,"Times New Roman",Times,serif,sans-serif;font-size:12px;color:#222}
        body{margin:20px}
        .space{margin-bottom:5px}
        .alert{border:1px solid transparent;border-radius:4px!important;margin-bottom:6px!important;padding:4px!important}
        .weak{color:#999;font-size:14px}
        .labelweak{color:#5f6162;font-size:12px;margin:5px -6px 3px 8px}
        .required:after{content:" *";font-weight:700;color:#c1c1c1;font-size:20px;margin:0;float:right}
        .windowContainer{width:400px;padding:0;background:#fefefe;margin:0 auto;border:1px solid #c4cddb;border-top-color:#d3dbde;border-bottom-color:#bfc9dc;box-shadow:0 1px 1px #ccc;border-radius:5px;position:relative}
        .windowContainer h3{margin:0;padding:10px 0;font-size:16px;text-align:center;background:#ebebec;border-bottom:1px solid #dde0e7;box-shadow:0 -1px 0 #fff inset;border-radius:5px 5px 0 0;text-shadow:1px 1px 0 #fff;font-weight:700}
        .windowContainer ul,li{margin:0;padding:0;list-style-type:square}
        .windowContainer input{border:1px solid #d5d9da;border-radius:5px;box-shadow:0 0 5px #e8e9eb inset;outline:0}
        .fieldSetWithBorder{border:2px dashed #eee;margin:2px 2px 3px 3px;overflow:hidden;padding:2px;width:217px}
        .fieldSetWithBorder legend{font-size:14px;line-height:inherit}
        #passwordCriteriaContainer{background:#fefefe;border:1px solid #ddd;border-radius:5px;box-shadow:0 1px 3px #ccc;display:none;font-size:.8em;padding:6px;position:absolute;right:41px;width:236px;z-index:2000;left:30%;top:68%}
        #passwordCriteriaContainer:before{content:"\25B2";position:absolute;top:-10px;left:45%;font-size:14px;line-height:12px;color:#ddd;text-shadow:none;display:block}
        #passwordCriteriaContainer h1,h2,h3,h4{margin:0 0 10px;padding:0;font-weight:400}
        #passwordCriteriaContainer ul,li{list-style-type:circle;margin:4px 3px 4px 4px;padding:2px 1px 3px 9px;width:203px}
        .invalidPassword,.validPassword{padding-left:6px;line-height:12px}
        .invalidPassword{background:url(images/invalid.png) 0 50% no-repeat;color:#ec3f41}
        .validPassword{background:url(images/valid.png) 0 50% no-repeat;color:#3a7d34}
        .badMatch{color:#f66}
        .textbox-focus{border:solid 2px #00C;background:#def}
        input.textbox-focus{border:2px solid #00C;background:#def}
        .ddl{border:2px solid #d5d9da;border-radius:3px;padding:3px;text-indent:.01px;font-size:13px;font-family:Georgia;margin:6px 6px 0 2px}
        .dashedSeparator{margin:25px 0;border-bottom:dashed 1px #666}
        /*bootstra specifc*/
        .container{width:832px}
        td{padding:4px!important}
        .form-control{height:30px}
        .table{margin-bottom:5px}
        /*password hint*/
        .smallContainer{background:#F4F9FB none repeat scroll 0 0;border:2px dashed #ddd;margin:1px 4px -7px -1px;width:400px}
        .first-of-type{margin:4px 3px 4px -2px}
        .addBar{font-size:14px;font-weight:700;margin:-1px 36px -3px;overflow:hidden;width:300px}
        .listUL{font-size:12px;list-style-type:square;margin:3px 0 -2px 32px;padding:2px 1px 1px 6px;width:312px}
        .listLi{font-size:12px;list-style-type:square;margin:3px 0 -2px 32px;padding:2px 1px 1px 6px;width:267px}
        .showHideDivHeader{width:400px;padding:0;margin:4px 8px 9px 1px;cursor:pointer}
        .showHideDivContext{padding:5px -1px;background-color:#fafafa}
        .showPassword{float:right;margin:-25px 13px 0 0}
        .hidePassword{float:right;margin:-30px 2px 0 0;background:#ff0 url(./Images/eye-icon.png) 20px 75px no-repeat}
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
        <%--for responsive design--%>
        <div class="container-fluid">
            <div class="row">
                <div class="col-sm-6">
                    <%--window for the user profile info--%>
                    <div id="divUserProfileSettingsContainer" class="windowContainer">
                        <h3>User Profile Information</h3>
                        <table class="table table-hover">
                            <tbody>
                                <tr>
                                    <td>
                                        <label for="UserFirstName" class="labelweak">First Name:</label></td>
                                    <td>
                                        <asp:TextBox ID="UserFirstName" runat="server" CssClass="form-control" Width="185px" MaxLength="50"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td>
                                        <label for="UserLastName" class="labelweak">Last Name:</label></td>
                                    <td>
                                        <asp:TextBox ID="UserLastName" runat="server" CssClass="form-control" Width="185px" MaxLength="50"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td>
                                        <label for="Email" class="labelweak required">Email Address:&nbsp;</label></td>
                                    <td>
                                        <asp:TextBox ID="Email" runat="server" CssClass="form-control" Width="250px" MaxLength="256"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td>
                                        <label for="Phone" class="labelweak required">Phone:&nbsp;</label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="Phone" runat="server" CssClass="form-control phone" Width="185px" MaxLength="10"></asp:TextBox>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                    <%--window for the user profile info--%>
                    <div class="space"></div>
                    <%--password hint--%>
                    <div id="divPasswordHint" class="showHideDivHeader" runat="server">
                        <span class="heading weak">Password Hint&nbsp;<img id="imgShow" src="~/Images/InformationBulb.png" runat="server"
                            width="18" height="18" />
                        </span>
                        <div class="showHideDivContext">
                            <div class="smallContainer">
                                <fieldset class="first-of-type">
                                    <legend id="litLegend" class="addBar">Your password must:</legend>
                                    <ul class="listUL">
                                        <li class="listLi">Be at least 8 characters long</li>
                                        <li class="listLi">Conatin at least one letter [a-z or A-Z]</li>
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
                <div class="space"></div>
                <div class="col-sm-5">
                    <%--window for the user Password Verification info--%>
                    <div class="windowContainer">
                        <h3>Password Verification</h3>
                        <table class="table table-hover">
                            <tbody>
                                <tr>
                                    <td>
                                        <label for="CurrentPassword" class="labelweak required">Current Password:&nbsp;</label></td>
                                    <td>
                                        <asp:TextBox ID="CurrentPassword" runat="server" class="form-control" TextMode="Password" Width="185px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label for="NewPassword" class="labelweak required">New Password:&nbsp;</label></td>
                                    <td>
                                        <%--ensure only 8 chars are entered--%>
                                        <asp:TextBox ID="NewPassword" runat="server" class="NewPassword form-control" TextMode="Password" Width="185px"
                                            MaxLength="8"></asp:TextBox>
                                                <img src="Images/eye-icon.png" class="showPassword" 
                                                    id="imageShowPassword" onclick="changeImage(this)" width="18" height="18"/>

                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label for="ConfirmNewPassword" class="labelweak required">Confirm Password:&nbsp;</label></td>
                                    <td>
                                        <asp:TextBox ID="ConfirmNewPassword" runat="server" class="form-control" TextMode="Password" Width="185px"
                                            MaxLengt="8"
                                            onkeypress="return this.value.length<=7"></asp:TextBox>
                                        <span id="passwordMatchMessage" class="passwordMatchMessage"></span>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                        <div id="passwordCriteriaContainer">
                            <fieldset class="fieldSetWithBorder">
                                <legend>Password Criteria</legend>
                                <div id="divcriteria" style="margin: -15px 0 0 1px;">
                                    <ul>
                                        <li id="letter" class="invalidPassword">At least <strong>one letter</strong></li>
                                        <li id="capital" class="invalidPassword">At least <strong>one capital letter</strong></li>
                                        <li id="lowercase" class="invalidPassword">At least <strong>one lowercase letter</strong></li>
                                        <li id="number" class="invalidPassword">At least <strong>one number</strong></li>
                                        <li id="special" class="invalidPassword">Must contain a <strong>special character</strong></li>
                                        <li id="length" class="invalidPassword">Be at least <strong>8 characters</strong></li>
                                    </ul>
                                </div>
                            </fieldset>
                        </div>
                    </div>
                    <%--window for the user Password Verification info--%>
                    <div class="space"></div>
                    <%--window for the user Password Recovery Settings--%>
                    <div class="windowContainer">
                        <h3>Password Recovery Settings</h3>
                        <table class="table table-hover">
                            <tbody>
                                <tr>
                                    <td>
                                        <label for="SecretPhraseDropdown" class="labelweak">Security Question:</label></td>
                                    <td>
                                        <asp:DropDownList ID="SecretPhraseDropdown" runat="server" Width="293px"
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
                                        <asp:TextBox ID="Answer" runat="server" CssClass="form-control" Width="295px" MaxLength="128"></asp:TextBox></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                    <%--window for the user Password Recovery Settings--%>
                </div>
            </div>
            <div class="space"></div>
            <div id="alert_placeholder" class="errorAlert"></div>
            <div class="space"></div>
            <div class="row">
                <div class="center-block" style="float: none; padding: 4px; width: 415px;">
                    <div class="alert alert-warning">
                        <strong>Important!</strong> All required fields are marked with an asterisk (*).
                    </div>
                    <div class="center-block col-md-8" style="float: none;">
                        <asp:Button ID="Button1" runat="server" CommandName="ChangePassword" Text="Apply Changes"
                            OnClick="ChangePasswordPushButton_Click" CssClass="btn btn-primary"
                            OnClientClick="return Validate();" />
                        <asp:Button ID="Button2" runat="server" CommandName="ResetForm" Text="Reset Form" CssClass="btn btn-primary"
                            OnClientClick="ClearTextboxes();" />
                    </div>
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
    <script type="text/javascript">
        var GlobalError = '';
        var PasswordFormatError = "Passwords must have a length of 7 characters, with at least 1 non-alphanumeric character.";
       
        //form load
        function OnLoad() {
            if (window.location.href.indexOf('newuser') == -1) {
                //do nothing
            }
            else {
                bootstrap_alert.warning("You are required to change your password and complete your profile information before continuing.&nbsp <br>Passwords must have a minimum length of 7 characters with at least 1 non-alphanumeric character.")
            }
        }

        $(".showHideDivContext").hide();
        //toggle the div
        $(".heading").click(function () {
            $(this).next(".showHideDivContext").slideToggle(500);
        });

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

        //init Bootstrap alert//
        bootstrap_alert = function () { }
        bootstrap_alert.warning = function (message) {
            $('#alert_placeholder').html('<div class="alert alert-danger fade in"><strong> &nbsp Error&nbsp! </strong>&nbsp&nbsp' + message
                                        + '<a class="close" data-dismiss="alert"><b>x</b></a></div>')
        }

        //alert close
        $(".errorAlert").click(function () {
            $(".alert-danger").alert("close");
        });

        //Prevent cut, copy, paste
        $('#ConfirmNewPassword').bind("cut copy paste", function (e) {
            e.preventDefault();
        });

        $("#ConfirmNewPassword").keyup(validatePasswordMatch);

        //format phone number to (000)000-0000
        // check for char and symbos. allow only numbers
        function phoneFormatter() {
            $('.phone').on('input', function () {
                var number = $(this).val().replace(/[^\d]/g, '')
                if (number.length == 7) {
                    number = number.replace(/(\d{3})(\d{4})/, "$1-$2");
                } else if (number.length == 10) {
                    number = number.replace(/(\d{3})(\d{3})(\d{4})/, "($1)$2-$3");
                }
                $(this).val(number)
            });
        };

        $(phoneFormatter);

        //function to validate data
        function Validate() {
            try {

                //allow only alphabets
                var regixCharsOnly = new RegExp(/^[a-zA-Z]*$/);
                if ($('#UserFirstName').val() != '') {
                    if (!$('#UserFirstName').val().match(regixCharsOnly)) {
                        bootstrap_alert.warning('The First name can be characters only.');
                        $("#UserFirstName").focus();
                        return false;
                    }
                }
                if ($('#UserLastName').val() != '') {
                    if (!$('#UserLastName').val().match(regixCharsOnly)) {
                        bootstrap_alert.warning('The last name can be characters only.');
                        $("#UserLastName").focus();
                        return false;
                    }
                }

                var MyEmail = $('#Email').val();
                if (MyEmail == '') {
                    bootstrap_alert.warning('The Email field is required.');
                    $("#Email").focus();
                    return false;
                }

                var Phone = $('#Phone').val();
                if (Phone == '') {
                    bootstrap_alert.warning('The Phone field is required.');
                    $('#Phone').focus();
                    return false;
                }
                if ((Phone != '') && (IsValidPhoneNumber(Phone) == false)) {
                    bootstrap_alert.warning(GlobalError);
                    $('#Phone').focus()
                    $('#Phone').select()
                    return false;
                }

                var CurPassword = $('#CurrentPassword').val(); //document.getElementById("CurrentPassword").value;
                var newPassword = $('#NewPassword').val(); //document.getElementById("NewPassword").value;
                var confirmPassword = $('#ConfirmNewPassword').val(); //document.getElementById("ConfirmNewPassword").value;

                if (CurPassword == '') {
                    bootstrap_alert.warning('The password setting Current Password filed cannot be empty.');
                    $("#CurrentPassword").focus()
                    return false;
                }

                if (newPassword == '') {
                    bootstrap_alert.warning('The password setting New Password filed cannot be empty.');
                    $("#NewPassword").focus()
                    return false;
                }

                if (newPassword.length < 8) {
                    bootstrap_alert.warning('The New Password field length must be 8 chars.');
                    $("#NewPassword").focus()
                    return false;
                }

                if (confirmPassword == '') {
                    bootstrap_alert.warning('The password setting Confirm New Password filed cannot be empty.');
                    $('#ConfirmNewPassword').focus()
                    return false;
                }

                if (confirmPassword.length < 8) {
                    bootstrap_alert.warning('The Confirm Password length must be 8 chars.');
                    $('#ConfirmNewPassword').focus()
                    return false;
                }

                if (CurPassword != '') {
                    if (newPassword != confirmPassword) {
                        bootstrap_alert.warning('The password setting New Password filed and Confirm New Password field do not match!');
                        $("#NewPassword").focus()
                        $("#NewPassword").select()
                        return false;
                    }
                }

                var passwordAnswer = $('#Answer').val(); //document.getElementById("Answer").v;
                if (passwordAnswer == '') {
                    bootstrap_alert.warning('The password retrieval setting Answer field is required.');
                    $('#Answer').focus()
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

            //validate any letter
            if (newpasswordValue.match(/[A-z]/)) {
                $('#letter').removeClass('invalidPassword').addClass('validPassword');
            } else {
                $('#letter').removeClass('validPassword').addClass('invalidPassword');
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
                alert('The character you entered is not valid.');
                return false;
            }
            //not allowed continus repeated chars
            var regex = new RegExp(/([A-Za-z])\1\1\1/);
            if (newpasswordValue.match(regex)) {
                alert('You can not have more than 3 continus repeated chars.');
                return false;
            }


            //validate non-printable chars 
            if (newpasswordValue.match(/[^\u0000-\u007F]/)) {
                bootstrap_alert.warning('You can not have non-printable chars.');
                return false;
            }
        }).focus(function () {
            $('#passwordCriteriaContainer').show();
        }).blur(function () {
            $('#passwordCriteriaContainer').hide();
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
    </script>
</body>
</html>
