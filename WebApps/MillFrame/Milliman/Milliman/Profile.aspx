<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="MillimanDev2.Profile" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>User Profile/Password Settings</title>
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <link id="lnkBootstrapcss" runat="server" rel="stylesheet" type="text/css" href="~/Content/Style/bootstrap.css" />
    <link id="Link1" runat="server" rel="stylesheet" type="text/css" href="~/Content/Style/MillframeStyle.css" />
    <style type="text/css">
        html {
            overflow: scroll;
        }

        html, button, input, select, textarea, label {
            font-family: arial,"Times New Roman",Times,serif,sans-serif;
            font-size: 12px;
            color: #222;
        }

        body {
            margin: 20px;
        }
    </style>

    <script src="Content/Script/jquery.v1.7.1.js"></script>
    <script type="text/javascript">
        function getRadWindow() {
            var oWindow = null;
            if (window.radWindow)
                oWindow = window.radWindow;
            else if (window.frameElement.radWindow)
                oWindow = window.frameElement.radWindow;
            return oWindow;
        };

        // Reload parent page
        function CloseDialog() {
            var ThisDialog = getRadWindow();
            var Parent = getRadWindow().BrowserWindow;
            Parent.radalert('Profile/Password information has been saved.', 350, 150, "Information Saved");
            ThisDialog.close();
        };

    </script>
</head>
<body style="background-color: white;" onload="OnLoad();">
    <form id="form1" runat="server">
        <div class="containerWrap">
            <div class="page-header engravedHeader">
                <h2>User Profile  <small>Password Settings</small></h2>
            </div>
            <div class="space"></div>
            <%--divImportantHint hint--%>
            <div id="divImportant" class="divImportant" style="width: 23px; float: left;">
                <img id="img2" src="~/Content/Images/Info-blue.png" runat="server" width="18" height="18" style="margin: 2px 6px 6px 2px;" />
                <div id="divImportantHint">
                    <div class="alert alert-warning infoBox text-justify">
                        <strong>Important!</strong>
                        <br />
                        Please complete all required fields marked with an asterisk (*).
                     <%--Allowed special Chars--%>
                        <div id="divSpecialChars">
                            Allowed Special Characters in name:&nbsp;&nbsp;<asp:Label ID="lblAllowedChars" runat="server"></asp:Label>
                        </div>
                    </div>
                </div>
            </div>
            <%--divImportantHint hint--%>
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
                                    <input id="UserFirstName" name="UserFirstName" type="text" runat="server" class="form-control"
                                        placeholder="first name..." maxlength="50" style="width: 185px;" tabindex="1"
                                        onclick="this.select();" />
                                    <asp:HiddenField ID="hdAllChars" runat="server" />
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
                                    <label for="Phone" class="labelweak">Phone:&nbsp;</label>
                                </td>
                                <td>
                                    <input id="Phone" name="Phone" type="text" runat="server" class="form-control phone" placeholder="(000)000-0000"
                                        style="width: 185px;" tabindex="3" />
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                <%--window for the user profile info--%>
                <div id="divHints">
                    <%--password hint--%>
                    <div id="divPasswordHint" class="divPasswordHintHeader">
                        <span class="weak">Password Hint&nbsp;<img id="imgShow" src="~/Content/Images/InformationBulb.png" runat="server"
                            width="18" height="18" style="margin: 2px 6px 6px 2px;" />
                        </span>
                        <div id="divPasswordHintCriteria">
                            <div class="divPasswordHintSmallContainer">
                                <fieldset class="first-of-type">
                                    <legend id="litLegend" class="addBar">Your password must:</legend>
                                    <ul class="listUL">
                                        <li class="listLi">Be at least 8 characters long</li>
                                        <li class="listLi">Contain a capital letter [A-Z]</li>
                                        <li class="listLi">Contain a lowercase letter [a-z]</li>
                                        <li class="listLi">Contain a number [0-9]</li>
                                        <li class="listLi">Contain a special character
                                            <asp:Label ID="lblAllowedSpecialCharactersInPassword" runat="server"></asp:Label></li>
                                    </ul>
                                </fieldset>
                            </div>
                        </div>
                    </div>
                </div>
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
                                    <asp:TextBox ID="CurrentPassword" runat="server" class="form-control" TextMode="Password" Width="185px"
                                        onclick="this.select();">
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <label for="NewPassword" class="labelweak required">New Password:&nbsp;</label></td>
                                <td>
                                    <input id="NewPassword" name="NewPassword" type="password" runat="server" class="NewPassword form-control"
                                        style="width: 185px;" tabindex="5" onclick="this.select();" maxlength="38" />
                                    <img src="Content/Images/eye-icon.png" class="showPassword"
                                        id="imageShowPassword" onclick="changeImage(this)" width="18" height="18" />
                                    <div id="divPasswordCriteriaContainer" class="passwordCriteria roundShadowContainer">
                                        <fieldset class="fieldSetWithBorder">
                                            <legend>Password Criteria</legend>
                                            <div id="divcriteria" style="margin: -15px 0 0 1px;">
                                                <ul class="orderedListTypePasswordCriteria">
                                                    <li id="capital" class="invalidPassword orderedListTypePasswordCriteria">At least <strong>one capital letter</strong></li>
                                                    <li id="lowercase" class="invalidPassword orderedListTypePasswordCriteria">At least <strong>one lowercase letter</strong></li>
                                                    <li id="number" class="invalidPassword orderedListTypePasswordCriteria">At least <strong>one number</strong></li>
                                                    <li id="special" class="invalidPassword orderedListTypePasswordCriteria">Must contain a <strong>special character</strong></li>
                                                    <li id="length" class="invalidPassword orderedListTypePasswordCriteria">Be at least <strong>8 characters</strong></li>
                                                </ul>
                                            </div>
                                        </fieldset>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <label for="ConfirmNewPassword" class="labelweak required">Confirm New Password:&nbsp;</label></td>
                                <td>
                                    <input id="ConfirmNewPassword" name="ConfirmNewPassword" type="password" runat="server" class="form-control"
                                        style="width: 185px;" tabindex="6" onclick="this.select();" maxlength="38" />

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
                                        style="width: 295px;" tabindex="7" maxlength="128" onclick="this.select();" />
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                <%--window for the user Password Recovery Settings--%>
            </div>
            <div class="space"></div>
            <div class="row">
                <div class="center-block" style="float: none; width: 415px;">
                    <div class="col-md-12">
                        <asp:Button ID="Button1" runat="server" CommandName="ChangePassword" Text="Save"
                            OnClick="ChangePasswordPushButton_Click" CssClass="btn btn-primary"
                            OnClientClick="return Validate();" />
                    </div>
                </div>
            </div>
        </div>

        <div id="footer" style="height: 25px; bottom: 0; position: absolute; left: 10px; right: 10px; overflow: hidden; vertical-align: bottom; display: none">
            <table style="width: 100%; height: 100%; overflow: hidden">
                <tr>
                    <td valign="middle">
                        <center> <div>
                    Copyright © Milliman &nbsp
                                <asp:Label ID="lblcopyrightYear" runat="server"></asp:Label>
                    <script type="text/javascript">document.getElementById("lblcopyrightYear").innerHTML = new Date().getFullYear();</script>
                </div></center>
                    </td>
                </tr>
            </table>
        </div>

    </form>

    <script src="Content/Script/jquery.v1.9.1.js"></script>
    <script src="Content/Script/jquery.min.v2.1.1.js"></script>
    <script src="Content/Script/bootstrap.js"></script>
    <script src="Content/Script/bootstrap-dialog.min.js"></script>
    <script src="Content/Script/maskedinput.js" type="text/javascript"></script>
    <link href="Content/Style/bootstrap-dialog.min.css" rel="stylesheet" />

    <script type="text/javascript">

        var passwordMinimumLength = 8;

        //form load
        function OnLoad() {
            var msg = 'You are required to change your password and complete your profile information before continuing.&nbsp <br>Passwords must comply with the indicated requirements.';
            if (window.location.href.indexOf('newuser') == -1) {
                //do nothing   -- this is not a new user
            }
            else {
                showInformationAlert(msg);
            }
        }

        //***************** Clear the Text Boxes ******************************//
        function ClearTextboxes() {
            try {
                document.getElementById('UserFirstName').value = '';
                document.getElementById('UserLastName').value = '';
                document.getElementById('Phone').value = '';
                document.getElementById('CurrentPassword').value = '';
                document.getElementById('NewPassword').value = '';
                document.getElementById('ConfirmNewPassword').value = '';
                document.getElementById('Answer').value = '';

                //select first element in drop down list
                var ddl = $('select[name="SecretPhraseDropdown"]');
                ddl.val(ddl.find('option').first().val());
            }
            catch (err) {
                var txt = 'Error=>' + err.description;
                showDangerAlert(txt);
            }
        }
        //*****************  Common ******************************//

        //set phone mask
        $("#Phone").mask("(999) 999-9999");

        //turn off autocomplete
        // Opera 8.0+
        var isOpera = (!!window.opr && !!opr.addons) || !!window.opera || navigator.userAgent.indexOf(' OPR/') >= 0;
        // Firefox 1.0+
        var isFirefox = typeof InstallTrigger !== 'undefined';
        // Safari <= 9 "[object HTMLElementConstructor]"
        var isSafari = Object.prototype.toString.call(window.HTMLElement).indexOf('Constructor') > 0;
        // Internet Explorer 6-11
        var isIE = /*@cc_on!@*/false || !!document.documentMode;
        // Edge 20+
        var isEdge = !isIE && !!window.StyleMedia;
        // Chrome 1+
        var isChrome = !!window.chrome && !!window.chrome.webstore;
        // Blink engine detection
        var isBlink = (isChrome || isOpera) && !!window.CSS;

        if (isChrome || isIE || isEdge || isFirefox || isSafari || isOpera) {
            $('input[name="CurrentPassword"]').attr('autocomplete', 'off');
            $('input[name="NewPassword"]').attr('autocomplete', 'off');
            $('input[name="ConfirmNewPassword"]').attr('autocomplete', 'off');
        }

        //***************** display the importatn hint next to the image ******************************//
        var moveLeft = 10;
        var moveDown = 20;
        $('.divImportant').hover(function (e) {
            $('#divImportantHint').show();
        }, function () {
            $('#divImportantHint').hide();
        });

        $('.divImportant').mousemove(function (e) {
            $("#divImportantHint")
                .css('top', e.pageY + moveDown)
                .css('left', e.pageX + moveLeft);
        });

        //***************** display the password hint next to the image ******************************//
        var moveLeft = 10;
        var moveDown = 20;
        $('.divPasswordHintHeader').hover(function (e) {
            $('#divPasswordHintCriteria').show();
        }, function () {
            $('#divPasswordHintCriteria').hide();
        });

        $('.divPasswordHintHeader').mousemove(function (e) {
            $("#divPasswordHintCriteria")
                .css('top', e.pageY + moveDown)
                .css('left', e.pageX + moveLeft);
        });

        //***************** show the password criteria next to input box ******************************//
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

        //***************** switch between show hide image ******************************//
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

        //***************** Change Image ******************************//
        function changeImage(element) {
            var right = "./Content/Images/eye-icon.png";
            var left = "./Content/Images/eye-hide.png";
            element.src = element.bln ? right : left;
            element.bln = !element.bln;
        }

        //***************** Allowed Special Characters in Password ******************************//
        //if there are values for allwoed character then display it
        var lblAllowedSpecialCharactersInPassword = "<%= System.Configuration.ConfigurationManager.AppSettings["AllowedSpecialCharactersInPassword"].ToString() %>"
        if (lblAllowedSpecialCharactersInPassword.length > 1) {
            document.getElementById("lblAllowedSpecialCharactersInPassword").innerText = trim(lblAllowedSpecialCharactersInPassword);
        }

        //***************** Allowed Special Characters in Name ******************************//
        //if there are values for allwoed character then display it
        var AllowedSpecialCharactersInUserName = "<%= System.Configuration.ConfigurationManager.AppSettings["AllowedSpecialCharactersInUserName"].ToString() %>"
        if (AllowedSpecialCharactersInUserName.length > 1) {
            $('#divSpecialChars').show();
            var regex = new RegExp(',', 'g')
            document.getElementById("lblAllowedChars").innerText = AllowedSpecialCharactersInUserName.replace(regex, ' ');
        }
        else {
            $('#divSpecialChars').hide();
        }

        //***************** All Special Characters ******************************//
        //if there are values for allwoed character then display it
        var AllSpecialChars = "<%= System.Configuration.ConfigurationManager.AppSettings["AllSpecialChars"].ToString() %>"
        if (AllSpecialChars.length > 1) {
            $('#hdAllChars').val(AllSpecialChars);
        }

        //***************** Start User name checks ******************************//
        var UserFirstNameInput = document.getElementById("UserFirstName");
        if (UserFirstNameInput.addEventListener) {
            UserFirstNameInput.addEventListener("blur", verifyUserNameInput, false);
        }
        else {
            UserFirstNameInput.attachEvent("blur", verifyUserNameInput);
        }

        var UserLastInput = document.getElementById("UserLastName");
        if (UserLastInput.addEventListener) {
            UserLastInput.addEventListener("blur", verifyUserNameInput, false);
        }
        else {
            UserLastInput.attachEvent("blur", verifyUserNameInput);
        }

        function verifyUserNameInput(elementFocusEvent) {
            var elementID = elementFocusEvent.target.id;
            var elementValue = elementFocusEvent.target.value;

            CheckUserNameInput(elementValue, elementID);

            return true;
        }

        function CheckUserNameInput(elementValue, elementID) {
            var alphabets = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var number = "0123456789";
            var badData = false;
            //check for the non-numeric vals (alphabets)
            for (var i = 0; i < elementValue.length; i++) {
                if (!alphabets.indexOf(elementValue.charAt(i)) < 0) {
                    badData = true;
                }
            }
            //check for the numeric vals (1234)
            for (var i = 0; i < elementValue.length; i++) {
                if (number.indexOf(elementValue.charAt(i)) >= 0) {
                    badData = true;
                }
            }

            //if there are specail in name then check if they are allowed
            if (elementValue.match(new RegExp($('#hdAllChars').val(), "gi"))) {
                //allowed special chars from web.config
                var allowedSplChars = trim(AllowedSpecialCharactersInUserName).split(',');
                var allPresentCharactersInName = elementValue.match(new RegExp($('#hdAllChars').val(), "gi"));

                var found = [];
                var badFound = [];
                for (var i = 0; i < allPresentCharactersInName.length; i++) {
                    if (allowedSplChars.indexOf(allPresentCharactersInName[i]) !== -1) {
                        found.push(allPresentCharactersInName[i]);
                    }
                    else {
                        badFound.push(allPresentCharactersInName[i]);
                        badData = true;
                    }
                }
            }
            if (badData) {
                highlightUserNameInput(elementID);
                showErrorAlert(' You have entered invalid special characters ' + badFound + '. This field only allows alpahebts [a-z] and certian special characters like [' + AllowedSpecialCharactersInUserName + ']. Please re-enter valid data.');
                return false;
            }

            return true;
        }

        function highlightUserNameInput(elementID) {
            if (elementID == "UserFirstName") {
                $('#UserFirstName').addClass('textbox-focus');
            }
            if (elementID == "UserLastName") {
                $('#UserLastName').addClass('textbox-focus');
            }
        }
        //***************** End User name checks ******************************//

        //***************** Validate Data ******************************//
        function Validate() {
            try {
                var UserFirstNameInput = document.getElementById("UserFirstName");
                var UserLastInput = document.getElementById("UserLastName");
                //check if the names are goo
                var goodFirstName = CheckUserNameInput(UserFirstNameInput.value, UserFirstNameInput.id);
                if (goodFirstName) {
                    var goodUserLast = CheckUserNameInput(UserLastInput.value, UserLastInput.id);
                    //check if both names are good
                    if (!goodUserLast) {
                        return false;
                    }
                }
                else {
                    return false;
                }

                var CurPassword = $('#CurrentPassword').val();
                var newPassword = $('#NewPassword').val();
                var confirmPassword = $('#ConfirmNewPassword').val();

                //check the password only if CurPassword is entered
                if (CurPassword != '') {
                    if (newPassword == '') {
                        var msg = ('The password verification, New Password field cannot be empty.');
                        $('#NewPassword').addClass('textbox-focus');
                        showErrorAlert(msg);
                        return false;
                    }

                    if (newPassword.length < passwordMinimumLength) {
                        var msg = ('The New Password field length must be at least ' + passwordMinimumLength + ' chars.');
                        $('#NewPassword').addClass('textbox-focus');
                        showErrorAlert(msg);
                        return false;
                    }

                    if (confirmPassword == '') {
                        var msg = ('The password verification, Confirm New Password field cannot be empty.');
                        $('#ConfirmNewPassword').addClass('textbox-focus');
                        showErrorAlert(msg);
                        return false;
                    }

                    if (confirmPassword.length < passwordMinimumLength) {
                        var msg = ('The Confirm Password length must be at least ' + passwordMinimumLength + ' chars.');
                        $('#ConfirmNewPassword').addClass('textbox-focus');
                        showErrorAlert(msg);
                        return false;
                    }
                    if (newPassword != confirmPassword) {
                        var msg = ('The password verification, New Password field and Confirm New Password field do not match!');
                        showErrorAlert(msg);
                        $('#NewPassword').addClass('textbox-focus');
                        $("#NewPassword").select()
                        return false;
                    }
                }

                if ((newPassword != '') || (confirmPassword != '')) {
                    if (CurPassword == '') {
                        var msg = ('To change your password, your Current Password must be provided along with the requested New Password and Confirm New Password.');
                        $('#CurrentPassword').addClass('textbox-focus');
                        showErrorAlert(msg);
                        return false;
                    }
                }

                var passwordAnswer = $('#Answer').val();
                if (passwordAnswer == '') {
                    var msg = ('The password retrieval setting Answer field is required.');
                    $('#Answer').addClass('textbox-focus');
                    showErrorAlert(msg);
                    return false;
                }
            }
            catch (err) {
                return false;
                var txt = 'Error=>' + err.description;
                showDangerAlert(txt);
            }
            return true;
        }

        //***************** Start Validate Password Data ******************************//
        var badInputData = false;
        var messagePasswordUserNameChars = "";
        //Password Check
        $('input.NewPassword').keyup(function () {
            // set password variable
            var newpasswordValue = $(this).val();

            //validate the length
            if (newpasswordValue.length >= passwordMinimumLength) {
                $('#length').removeClass('invalidPassword').addClass('validPassword');
                badInputData = false;
            } else {
                $('#length').removeClass('validPassword').addClass('invalidPassword');
                badInputData = true;
            }

            //validate any uppercase letter
            if (newpasswordValue.match(/[A-Z]/)) {
                $('#capital').removeClass('invalidPassword').addClass('validPassword');
                if (badInputData) {
                    badInputData = true;
                }
                else {
                    badInputData = false;
                }
            } else {
                $('#capital').removeClass('validPassword').addClass('invalidPassword');
                badInputData = true;
            }

            //validate any lower case letter
            if (newpasswordValue.match(/[a-z]/)) {
                $('#lowercase').removeClass('invalidPassword').addClass('validPassword');
                if (badInputData) {
                    badInputData = true;
                }
                else {
                    badInputData = false;
                }
            } else {
                $('#lowercase').removeClass('validPassword').addClass('invalidPassword');
                badInputData = true;
            }

            //validate a number
            if (newpasswordValue.match(/[0-9]/)) {
                $('#number').removeClass('invalidPassword').addClass('validPassword');
                if (badInputData) {
                    badInputData = true;
                }
                else {
                    badInputData = false;
                }
            } else {
                $('#number').removeClass('validPassword').addClass('invalidPassword');
                badInputData = true;
            }

            //validate allowed special
            if (newpasswordValue.match(/[~!@#$%^&*;?+_.]/)) {
                $('#special').removeClass('invalidPassword').addClass('validPassword');
                if (badInputData) {
                    badInputData = true;
                }
                else {
                    badInputData = false;
                }
            } else {
                $('#special').removeClass('validPassword').addClass('invalidPassword');
                badInputData = true;
            }

            //validate non-printable chars
            if (newpasswordValue.match(/[^\u0000-\u007F]/)) {
                badInputData = true;
            }

            //the passwrod should not contain 3 or more char from user name
            //find user name
            var username = '<%=Context.User.Identity.Name%>';
            //get new password value
            var newPasswordVal = $('#NewPassword').val();
            //divide the user name into 3 letters so abcdefghi@somthing.com will look like [abd def ghi @som thi ng. com]
            // example: ["abc", "def", "g.h", "ijk", "@em", "ail", ".co", "m"]
            var partsOfThreeLettersUsernameArray = username.match(/.{3}/g)
                                .concat(
                                        username.substr(1).match(/.{3}/g),
                                        username.substr(2).match(/.{3}/g)
                                        );

            if (newPasswordVal != '' && newPasswordVal.length > 3) {
                //example: ["afs", "Ujn", "8*c", "fsU", "jn8", "*co", "sUj", "n8*", "com"]
                var partsOfThreeLettersPasswordArray = newPasswordVal.match(/.{3}/g)
                                        .concat(
                                                newPasswordVal.substr(1).match(/.{3}/g),
                                                newPasswordVal.substr(2).match(/.{3}/g)
                                                );


                var result = matchWordsinStringArray(partsOfThreeLettersUsernameArray, partsOfThreeLettersPasswordArray);
                if (result != null) {
                    //using (i) to Perform case-insensitive matching.
                    var elementValue = newPasswordVal.match(new RegExp(result.passwordElement, "i"));
                    //showErrorAlert('The password you entered cannot contain substring <b>' + elementValue[0] + '</b>, since <b>' + elementValue[0] + '</b> is a substring in your account name.  The password cannot contain 3 or more contiguous characters from the account name.');
                    messagePasswordUserNameChars = 'The password you entered cannot contain substring <b>' + elementValue[0] + '</b>, since <b>' + elementValue[0] + '</b> is a substring in your account name.  The password cannot contain 3 or more contiguous characters from the account name.';
                    badInputData = true;
                }
                else {
                    messagePasswordUserNameChars = "";
                    if (badInputData) {
                        badInputData = true;
                    }
                    else {
                        badInputData = false;
                    }

                }
            }

        }).focus(function () {
            $('#divPasswordCriteriaContainer').show();
        }).blur(function () {
            $('#divPasswordCriteriaContainer').hide();
        });

        function matchWordsinStringArray(usernameArray, passwordArray) {
            var arrayMatchfound = null;
            try {
                for (var i = 0; i < passwordArray.length && !arrayMatchfound; i++) {
                    var $lowerKeyPassword = passwordArray[i].toLowerCase();

                    for (var j = 0, wLen = usernameArray.length; j < wLen && !arrayMatchfound; j++) {
                        var $lowerKeyUserName = usernameArray[j].toLowerCase();

                        if ($lowerKeyPassword == $lowerKeyUserName) {
                            arrayMatchfound = {
                                usernameElement: $lowerKeyUserName,
                                passwordElement: $lowerKeyPassword
                            };
                            return arrayMatchfound;
                        }
                    }
                }

            }
            catch (err) {
                return false;
                var txt = 'Error=>' + err.description;
                showDangerAlert(txt);
            }

            return arrayMatchfound;
        }

        $('#ConfirmNewPassword').keyup(function () {
            var inputValue = $(this).val();
            //create regex
            var regex = /[`,<>;':"/[\]|{}()=-]/gi;
            var isSplChar = regex.test(inputValue);
            if (isSplChar) {
                //var removeSpecialChar = yourInput.replace(/[`~!@#$%^&*()_|+\-=?;:'",.<>\{\}\[\]\\\/]/gi, '');
                var removeSpecialChar = inputValue.replace(/[`()|:'",.<>\{\}\[\]\\\/]/gi, '');
                $(this).val(removeSpecialChar);
            }
        });
        $('#NewPassword').keyup(function () {
            var inputValue = $(this).val();
            //create regex
            var regex = /[`,<>;':"/[\]|{}()=-]/gi;
            var isSplChar = regex.test(inputValue);
            if (isSplChar) {
                //var removeSpecialChar = yourInput.replace(/[`~!@#$%^&*()_|+\-=?;:'",.<>\{\}\[\]\\\/]/gi, '');
                var removeSpecialChar = inputValue.replace(/[`()|:'",.<>\{\}\[\]\\\/]/gi, '');
                $(this).val(removeSpecialChar);
            }
        });

        var newPasswrdInput = document.getElementById("NewPassword");
        if (newPasswrdInput.addEventListener) {
            newPasswrdInput.addEventListener("blur", verifyBadPassword, false);
        }
        else {
            newPasswrdInput.attachEvent("blur", verifyBadPassword);
        }

        function verifyBadPassword() {

            var ConfirmNewPassword = $('#ConfirmNewPassword');
            var NewPassword = $('#NewPassword')
            if (NewPassword.val() === "") {
                NewPassword.removeClass('textbox-focus');
                ConfirmNewPassword.val('');
                resetPasswordMatch();
                ConfirmNewPassword.removeAttr('disabled');//enable
                return true;
            }
            else {
                if (badInputData) {

                    NewPassword.addClass('textbox-focus');
                    ConfirmNewPassword.val('');
                    resetPasswordMatch();

                    ConfirmNewPassword.attr('disabled', 'disabled');
                    if (messagePasswordUserNameChars != "") {
                        showErrorAlert(messagePasswordUserNameChars);
                        return false;
                    }
                    else {
                        showErrorAlert('Your password does not match all password rules. Please make sure your password matches the password rules. [Check the Password Hint.]');
                        return false;
                    }

                }
                else {
                    ConfirmNewPassword.removeAttr('disabled');//enable
                    ConfirmNewPassword.focus();
                    return true;
                }
            }
        }

        var ConfirmNewPasswordInput = document.getElementById("ConfirmNewPassword");
        if (ConfirmNewPasswordInput.addEventListener) {
            ConfirmNewPasswordInput.addEventListener("blur", validatePasswordMatch, false);
        }
        else {
            ConfirmNewPasswordInput.attachEvent("blur", validatePasswordMatch);
        }

        //function to check if the two password matches
        var message = document.getElementById('passwordMatchMessage');
        function validatePasswordMatch() {

            var newPassword = $("#NewPassword").val();
            var confirmPassword = $("#ConfirmNewPassword").val();

            if (newPassword === "" || confirmPassword === "") {
                resetPasswordMatch();
                return true;
            }

            if (newPassword != confirmPassword) {
                message.innerHTML = "Passwords Do Not Match!"
                $('#passwordMatchMessage').addClass('badMatch');
                return false;
            }

            if (newPassword === confirmPassword) {
                message.innerHTML = ""
            }
        }

        function resetPasswordMatch() {
            message.innerHTML = "";
            $('#passwordMatchMessage').removeClass('badMatch');
        }

        //***************** End  Validate Password Data ******************************//

        //***************** Alert Messages ******************************//
        function showErrorAlert(alertMessage) {
            BootstrapDialog.show({
                title: 'Data Entry Issue',
                message: alertMessage,
                type: BootstrapDialog.TYPE_WARNING, // <-- Default value is BootstrapDialog.TYPE_PRIMARY
                closable: true, // <-- Default value is false
                draggable: true, // <-- Default value is false
                buttons: [{
                    label: 'OK',
                    hotkey: 13, // Keycode of keyup event of key 'A' is 65.
                    cssClass: 'btn-warning',
                    action: function (dialog) {
                        dialog.close();
                    }
                }],
            });
        }

        //show error
        function showDangerAlert(alertMessage) {
            BootstrapDialog.show({
                title: 'Error!',
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
        //***************** Alert Messages ******************************//

        //UserFirstName.addEventListener("onclick", removeElementClass(), false);
        var UserFirstName = document.getElementById("UserFirstName");
        UserFirstName.onclick = function (event) {
            removeElementClass(UserFirstName, 'textbox-focus');
        };

        var UserLastName = document.getElementById("UserLastName");
        UserLastName.onclick = function (event) {
            removeElementClass(UserLastName, 'textbox-focus');
        };

        var Phone = document.getElementById("Phone");
        Phone.onclick = function (event) {
            removeElementClass(Phone, 'textbox-focus');
        };

        var CurrentPassword = document.getElementById("CurrentPassword");
        CurrentPassword.onclick = function (event) {
            removeElementClass(CurrentPassword, 'textbox-focus');
        };

        var NewPassword = document.getElementById("NewPassword");
        NewPassword.onclick = function (event) {
            removeElementClass(NewPassword, 'textbox-focus');
        };

        var ConfirmNewPassword = document.getElementById("ConfirmNewPassword");
        ConfirmNewPassword.onclick = function (event) {
            removeElementClass(ConfirmNewPassword, 'textbox-focus');
        };

        var Answer = document.getElementById("Answer");
        Answer.onclick = function (event) {
            removeElementClass(Answer, 'textbox-focus');
        };

        //***************** Element Class ******************************//

        //remove class
        // Check whether element has a classname
        function hasClass(ele, cls) {
            var clsChecker = new RegExp("\\b" + cls + "\\b");
            return clsChecker.test(ele.className);
        }
        // Add a classname
        function addClass(ele, cls) {
            var clsChecker = new RegExp("\\b" + cls + "\\b");
            if (clsChecker.test(ele.className)) {
                // ele already has the className, don't need to do anything
                return;
            }
            ele.className += (' ' + cls);
        }

        // Remove a classname
        function removeElementClass(ele, cls) {
            if (hasClass(ele, cls)) {
                var reg = new RegExp('(\\s|^)' + cls + '(\\s|$)');
                ele.className = ele.className.replace(reg, ' ');
            }
        }

        function ltrim(s) {
            var l = 0;
            while (l < s.length && s[l] == ' ')
            { l++; }
            return s.substring(l, s.length);
        }

        function rtrim(s) {
            var r = s.length - 1;
            while (r > 0 && s[r] == ' ')
            { r -= 1; }
            return s.substring(0, r + 1);
        }
        function trim(s) {
            return rtrim(ltrim(s));
        }
        //***************** Element Class ******************************//

    </script>
</body>
</html>
