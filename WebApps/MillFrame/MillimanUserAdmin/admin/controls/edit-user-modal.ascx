<%@ Control Language="C#" AutoEventWireup="true" CodeFile="edit-user-modal.ascx.cs" Inherits="admin_controls_edit_user_modal" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="~/js/js/jquery.ascx" TagName="jquery" TagPrefix="uc4" %>
<%@ Register Src="~/admin/controls/UserRolesSelector.ascx" TagName="userRoleSelector" TagPrefix="urs" %>

<link id="lnkBootstrapcss" runat="server" rel="stylesheet" type="text/css" href="../Content/Style/bootstrap.css" />
<link id="Link1" runat="server" rel="stylesheet" type="text/css" href="../Content/Style/MillframeStyle.css" />

<style type="text/css">
    .layOut {
        height: 135px !important;
    }

    #divPassword {
        border: 1px solid #eee;
    }
    /*bootstra specifc*/
    td {
        padding: 4px !important;
    }

    .form-control {
        height: 30px;
    }

    .table {
        margin-bottom: 5px;
    }

    .page-header {
        padding-bottom: 10px;
        margin: 5px 0 10px;
        border-bottom: 1px solid #eee;
    }
</style>

<div id="containerWrap">
    <%-- ajax update panel start --%>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <%-- ajax tab container start --%>
            <cc1:TabContainer ID="tcntUserInfo" runat="server" ActiveTabIndex="3" Width="100%" Font-Size="10px" CssClass="aTab1">
                <cc1:TabPanel ID="TabPanel2" runat="server" HeaderText="General User Info">
                    <ContentTemplate>
                        <div class="roundShadowContainer">
                            <urs:userRoleSelector ID="ctrlUserRoles" runat="server" />
                        </div>
                        <div class="space"></div>
                        <div class="roundShadowContainer">
                            <div class="page-header engravedHeader">
                                <h2>User Info</h2>
                            </div>
                            <div class="space"></div>
                            <asp:DetailsView AutoGenerateRows="False" DataSourceID="MemberData" ID="UserInfo" runat="server"
                                OnItemUpdating="UserInfo_ItemUpdating" DefaultMode="Edit"
                                CssClass="table table-hover" GridLines="None" OnItemCreated="UserInfo_ItemCreated">
                                <Fields>
                                    <asp:BoundField DataField="UserName" HeaderText="User Name" ReadOnly="True" ControlStyle-CssClass="form-control"></asp:BoundField>
                                    <asp:BoundField DataField="Email" HeaderText="Email">
                                        <ControlStyle Width="300px" CssClass="form-control" />
                                    </asp:BoundField>
                                    <asp:TemplateField HeaderText="Security Q&A">
                                        <EditItemTemplate>
                                            <asp:TextBox ID="TextBox1" CssClass="form-control" runat="server" Text='<%# Bind("PasswordQuestion") + ":" + Bind("Comment")  %>' TextMode="MultiLine" Height="100px" Width="245px" ReadOnly="True"></asp:TextBox>
                                        </EditItemTemplate>
                                        <InsertItemTemplate>
                                            <asp:TextBox ID="TextBox1" CssClass="form-control" runat="server" Text='<%# Bind("PasswordQuestion") + ":" + Bind("Comment") %>' ReadOnly="True"></asp:TextBox>
                                        </InsertItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label1" runat="server" Text='<%# Bind("PasswordQuestion") + ":" + Bind("Comment")  %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Client User Administrator" ControlStyle-Width="300px">
                                        <ItemTemplate>
                                            <asp:CheckBox runat="server" ID="IsClientAdministrator" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Client Publishing Administrator">
                                        <ItemTemplate>
                                            <asp:CheckBox runat="server" ID="IsPublishingAdministrator" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Active User">
                                        <ItemTemplate>
                                            <asp:CheckBox ID="IsApproved" runat="server" Checked='<%# Bind("IsApproved") %>'
                                                Enabled="false"></asp:CheckBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Is Locked Out">
                                        <ItemTemplate>
                                            <asp:CheckBox ID="IsLockedOut" runat="server" Checked='<%# Bind("IsLockedOut") %>' Enabled="false"></asp:CheckBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Is Online">
                                        <ItemTemplate>
                                            <asp:CheckBox ID="IsOnline" runat="server" Checked='<%# Bind("IsOnline") %>' Enabled="false"></asp:CheckBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="CreationDate" HeaderText="Creation Date" ReadOnly="True" ControlStyle-CssClass="form-control"></asp:BoundField>
                                    <asp:BoundField DataField="LastActivityDate" HeaderText="Last Activity Date" ReadOnly="True" ControlStyle-CssClass="form-control"></asp:BoundField>
                                    <asp:BoundField DataField="LastLoginDate" HeaderText="Last Login Date" ReadOnly="True" ControlStyle-CssClass="form-control"></asp:BoundField>
                                    <asp:BoundField DataField="LastLockoutDate" HeaderText="Last Lockout Date" ReadOnly="True" ControlStyle-CssClass="form-control"></asp:BoundField>
                                    <asp:BoundField DataField="LastPasswordChangedDate" HeaderText="Last Password Changed Date" ReadOnly="True" ControlStyle-CssClass="form-control"></asp:BoundField>
                                    <asp:TemplateField ShowHeader="False">
                                        <EditItemTemplate>
                                            <asp:Button ID="Button1" CssClass="btn btn-primary" runat="server" CausesValidation="True" CommandName="Update" Text="Save" OnClientClick="return confirm('This will UPDATE the User Info. Click OK to continue.')" />
                                            <asp:Button ID="Button2" CssClass="btn btn-primary" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel" />
                                            <asp:Button ID="Button4" CssClass="btn btn-primary" runat="server" Text="Unlock" OnClick="UnlockUser" OnClientClick="return confirm('Click OK to unlock this user.')" />
                                            <asp:Button ID="Button5" CssClass="btn btn-primary" runat="server" Text="Delete" OnClick="DeleteUser" OnClientClick="return confirm('Are you sure? This will delete all information related to this user including the user profile.')" />
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Button ID="Button1" CssClass="btn btn-primary" runat="server" CausesValidation="False" CommandName="Edit" Text="Edit User Info" />
                                        </ItemTemplate>
                                        <ControlStyle Font-Size="11px" />
                                    </asp:TemplateField>
                                </Fields>
                            </asp:DetailsView>
                            <div class="messageWrap2">
                                <asp:Literal ID="UserUpdateMessage" runat="server"></asp:Literal>
                            </div>
                            <br />
                            <asp:ObjectDataSource ID="MemberData" runat="server" DataObjectTypeName="System.Web.Security.MembershipUser" SelectMethod="GetUser" UpdateMethod="UpdateUser" TypeName="System.Web.Security.Membership">
                                <SelectParameters>
                                    <asp:QueryStringParameter Name="username" QueryStringField="username" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </div>
                    </ContentTemplate>
                </cc1:TabPanel>
                <cc1:TabPanel ID="TabPanel3" runat="server" HeaderText="User Profile" Visible="False" CssClass="softRoundContainerStyle backgroundColor">
                    <ContentTemplate>
                        <div class="contentTemplate">
                            <div class="formSectionTitle2">
                                USER DETAIL
                            </div>
                            <div class="formLabelsText">
                                First name:<br />
                                <asp:TextBox ID="txtFirstName" runat="server" Width="99%" MaxLength="50"></asp:TextBox>
                            </div>
                            <div class="formLabelsText">
                                Last name:<br />
                                <asp:TextBox ID="txtLastName" runat="server" Width="99%" MaxLength="50" />
                            </div>
                            <div class="formLabelsText">
                                Gender:<br />
                                <asp:DropDownList runat="server" ID="ddlGenders">
                                    <asp:ListItem Text="Please select one..." Selected="True" />
                                    <asp:ListItem Text="Male" Value="M" />
                                    <asp:ListItem Text="Female" Value="F" />
                                </asp:DropDownList>
                            </div>
                            <div class="formLabelsText">
                                Birth date:<br />
                                <asp:TextBox ID="txtBirthDate" runat="server" Width="99%"></asp:TextBox>
                                <cc1:CalendarExtender ID="txtBirthDate_CalendarExtender" runat="server" TargetControlID="txtBirthDate" Enabled="True">
                                </cc1:CalendarExtender>
                                <asp:CompareValidator runat="server" ID="valBirthDateFormat" ControlToValidate="txtBirthDate" SetFocusOnError="True" Display="Dynamic" Operator="DataTypeCheck" Type="Date" ErrorMessage="The format of the birth date is not valid." ValidationGroup="EditProfile">
                            <br />
                            The format of the birth date is not valid.
                                </asp:CompareValidator>
                                <cc1:MaskedEditExtender ID="MaskedEditExtender1" runat="server" TargetControlID="txtBirthDate" Mask="99/99/9999" MaskType="Date" CultureAMPMPlaceholder="" CultureCurrencySymbolPlaceholder="" CultureDateFormat="" CultureDatePlaceholder="" CultureDecimalPlaceholder="" CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True" />
                                <cc1:MaskedEditValidator ID="MaskedEditValidator1" runat="server" ControlToValidate="txtBirthDate" ControlExtender="MaskedEditExtender1" Display="Dynamic" TooltipMessage="Please enter a date" EmptyValueMessage="Date must be in correct format" InvalidValueMessage="This date is invalid" ErrorMessage="MaskedEditValidator1" />
                            </div>
                            <div class="formLabelsText">
                                Occupation:<br />
                                <asp:DropDownList ID="ddlOccupations" runat="server" Width="99%">
                                    <asp:ListItem Text="Please select one..." Selected="True" />
                                    <asp:ListItem Text="Academic" />
                                    <asp:ListItem Text="Accountant" />
                                    <asp:ListItem Text="Actor" />
                                    <asp:ListItem Text="Architect" />
                                    <asp:ListItem Text="Artist" />
                                    <asp:ListItem Text="Business Manager" />
                                    <asp:ListItem Text="Carpenter" />
                                    <asp:ListItem Text="Chief Executive" />
                                    <asp:ListItem Text="Cinematographer" />
                                    <asp:ListItem Text="Civil Servant" />
                                    <asp:ListItem Text="Coach" />
                                    <asp:ListItem Text="Composer" />
                                    <asp:ListItem Text="Computer programmer" />
                                    <asp:ListItem Text="Cook" />
                                    <asp:ListItem Text="Counsellor" />
                                    <asp:ListItem Text="Doctor" />
                                    <asp:ListItem Text="Driver" />
                                    <asp:ListItem Text="Economist" />
                                    <asp:ListItem Text="Editor" />
                                    <asp:ListItem Text="Electrician" />
                                    <asp:ListItem Text="Engineer" />
                                    <asp:ListItem Text="Executive Producer" />
                                    <asp:ListItem Text="Fixer" />
                                    <asp:ListItem Text="Graphic Designer" />
                                    <asp:ListItem Text="Hairdresser" />
                                    <asp:ListItem Text="Headhunter" />
                                    <asp:ListItem Text="HR - Recruitment" />
                                    <asp:ListItem Text="Information Officer" />
                                    <asp:ListItem Text="IT Consultant" />
                                    <asp:ListItem Text="Journalist" />
                                    <asp:ListItem Text="Lawyer / Solicitor" />
                                    <asp:ListItem Text="Lecturer" />
                                    <asp:ListItem Text="Librarian" />
                                    <asp:ListItem Text="Mechanic" />
                                    <asp:ListItem Text="Model" />
                                    <asp:ListItem Text="Musician" />
                                    <asp:ListItem Text="Office Worker" />
                                    <asp:ListItem Text="Performer" />
                                    <asp:ListItem Text="Photographer" />
                                    <asp:ListItem Text="Presenter" />
                                    <asp:ListItem Text="Producer / Director" />
                                    <asp:ListItem Text="Project Manager" />
                                    <asp:ListItem Text="Researcher" />
                                    <asp:ListItem Text="Salesman" />
                                    <asp:ListItem Text="Social Worker" />
                                    <asp:ListItem Text="Soldier" />
                                    <asp:ListItem Text="Sportsperson" />
                                    <asp:ListItem Text="Student" />
                                    <asp:ListItem Text="Teacher" />
                                    <asp:ListItem Text="Technical Crew" />
                                    <asp:ListItem Text="Technical Writer" />
                                    <asp:ListItem Text="Therapist" />
                                    <asp:ListItem Text="Translator" />
                                    <asp:ListItem Text="Waitress / Waiter" />
                                    <asp:ListItem Text="Web designer / author" />
                                    <asp:ListItem Text="Writer" />
                                    <asp:ListItem Text="Other" />
                                </asp:DropDownList>
                            </div>
                            <div class="formLabelsText">
                                Personal Website:<br />
                                <asp:TextBox ID="txtWebsite" runat="server" Width="99%" MaxLength="200" />
                            </div>
                            <div class="formSectionEnd">
                            </div>
                            <div class="formSectionTitle2">
                                ADDRESS
                            </div>
                            <div class="formLabelsText">
                                Country:<br />
                                <asp:DropDownList ID="ddlCountries" runat="server" AppendDataBoundItems="True" Width="99%">
                                    <asp:ListItem Selected="True" Text="Please select one..." />
                                </asp:DropDownList>
                            </div>
                            <div class="formLabelsText">
                                Address:<br />
                                <asp:TextBox runat="server" ID="txtAddress" Width="99%" MaxLength="100" />
                            </div>
                            <div class="formLabelsText">
                                Apartment Number:<br />
                                <asp:TextBox runat="server" ID="txtAptNumber" Width="99%" MaxLength="50" />
                            </div>
                            <div class="formLabelsText">
                                City:<br />
                                <asp:TextBox runat="server" ID="txtCity" Width="99%" MaxLength="100" />
                            </div>
                            <div class="formLabelsText">
                                State / Region:<br />
                                <asp:DropDownList ID="ddlStates1" runat="server" AppendDataBoundItems="True" Width="99%">
                                    <asp:ListItem Selected="True" Text="Please select one..." />
                                </asp:DropDownList>
                            </div>
                            <div class="formLabelsText">
                                Zip / Postal code:<br />
                                <asp:TextBox runat="server" ID="txtPostalCode" Width="99%" MaxLength="20" />
                            </div>
                            <div class="formSectionEnd">
                            </div>
                            <div class="formSectionTitle2">
                                CONTACT INFO
                            </div>
                            <div class="formLabelsText">
                                Day Time Phone:<br />
                                <asp:TextBox runat="server" ID="txtDayTimePhone" Width="99%" MaxLength="20" />
                            </div>
                            <div class="formLabelsText">
                                Day Time Phone Ext.:<br />
                                <asp:TextBox runat="server" ID="txtDayTimePhoneExt" Width="99%" MaxLength="10" />
                            </div>
                            <div class="formLabelsText">
                                Evening Phone:<br />
                                <asp:TextBox runat="server" ID="txtEveningPhone" Width="99%" MaxLength="20" />
                            </div>
                            <div class="formLabelsText">
                                Evening Phone Ext.:<br />
                                <asp:TextBox runat="server" ID="txtEveningPhoneExt" Width="99%" MaxLength="10" />
                            </div>
                            <div class="formLabelsText">
                                Cell Phone:<br />
                                <asp:TextBox runat="server" ID="txtCellPhone" Width="99%" MaxLength="20" />
                            </div>
                            <div class="formLabelsText">
                                Home Fax:<br />
                                <asp:TextBox runat="server" ID="txtHomeFax" Width="99%" MaxLength="20" />
                            </div>
                            <div class="formSectionEnd">
                            </div>
                            <div class="formSectionTitle2">
                                COMPANY DETAILS
                            </div>
                            <div class="formLabelsText">
                                Company Name:<br />
                                <asp:TextBox ID="txbCompanyName" runat="server" Width="99%" MaxLength="50"></asp:TextBox>
                            </div>
                            <div class="formLabelsText">
                                Address:<br />
                                <asp:TextBox ID="txbCompanyAddress" runat="server" Width="99%" MaxLength="50"></asp:TextBox>
                            </div>
                            <div class="formLabelsText">
                                City:<br />
                                <asp:TextBox ID="txbCompanyCity" runat="server" Width="99%" MaxLength="50"></asp:TextBox>
                            </div>
                            <div class="formLabelsText">
                                State:<br />
                                <asp:DropDownList ID="ddlStates2" runat="server" AppendDataBoundItems="True" Width="99%">
                                    <asp:ListItem Selected="True" Text="Please select one..." />
                                </asp:DropDownList>
                            </div>
                            <div class="formLabelsText">
                                Zip:<br />
                                <asp:TextBox ID="txbCompanyZip" runat="server" Width="99%" MaxLength="50"></asp:TextBox>
                            </div>
                            <div class="formLabelsText">
                                Phone:<br />
                                <asp:TextBox ID="txbCompanyPhone" runat="server" Width="99%" MaxLength="20"></asp:TextBox>
                            </div>
                            <div class="formLabelsText">
                                Fax:<br />
                                <asp:TextBox ID="txbCompanyFax" runat="server" Width="99%" MaxLength="20"></asp:TextBox>
                            </div>
                            <div class="formLabelsText">
                                Website:<br />
                                <asp:TextBox ID="txbCompanyWebsite" runat="server" Width="99%" MaxLength="200"></asp:TextBox>
                            </div>
                            <div class="formSectionEnd">
                            </div>
                            <div class="formSectionTitle2">
                                NEWSLETTER SUBSCRIPTION
                            </div>
                            <div class="formLabelsText">
                                Newsletter:<br />
                                <asp:DropDownList runat="server" ID="ddlNewsletter">
                                    <asp:ListItem Text="No subscription" Value="None" Selected="true" />
                                    <asp:ListItem Text="Subscribe to newsletter" Value="Html" />
                                </asp:DropDownList>
                            </div>
                            <div class="formSectionEnd">
                            </div>
                            <div class="formButton">
                                <asp:Button ID="btnUpdateProfile" CssClass="btn btn-primary" runat="server" Text="Update Profile" ValidationGroup="EditProfile" OnClick="btnUpdateProfile_Click" />
                                <asp:Button ID="btnDeleteProfile" CssClass="btn btn-primary" runat="server" OnClick="btnDeleteProfile_Click" OnClientClick="return confirm('Are Your Sure?')" Text="Delete Profile" />
                                &nbsp;
                                <asp:Label ID="lblProfileMessage" runat="server" />
                            </div>
                        </div>
                    </ContentTemplate>
                </cc1:TabPanel>
                <%--reset password tab--%>
                <cc1:TabPanel ID="TabPanel4" runat="server" HeaderText="Change Password" Font-Size="18px" CssClass="aTab1">
                    <HeaderTemplate>
                        Reset Password
                    </HeaderTemplate>
                    <ContentTemplate>
                        <div id="divPassword" class="roundShadowContainer">
                            <div class="page-header engravedHeader">
                                <h2>Reset Password</h2>
                            </div>
                            <div class="roundShadowContainer" style="visibility: visible">
                                <div class="row">&nbsp;</div>
                                <div class="row">
                                    <div class="col-sm-4">
                                        <asp:TextBox ID="OldPasswordTextbox" CssClass="form-control" runat="server" TextMode="Password" Width="140px" Visible="False"></asp:TextBox>
                                    </div>
                                    <div class="col-sm-8">
                                        <label for="PasswordTextbox" class="labelweak">Current Password:</label>
                                        <br />
                                        <asp:Label ID="lblCurrentPassword" runat="server" EnableViewState="False" ReadOnly="True" CssClass="form-control" Width="300px"></asp:Label>
                                    </div>
                                </div>
                                <div class="row">&nbsp;</div>
                                <div class="row">
                                    <div class="col-sm-4">
                                        <asp:Button ID="ChangePasswordButtonSecureLink" CssClass="btn btn-primary" runat="server" OnClick="ChangePasswordSecureLink_OnClick" Text="Reset Password&nbsp;(Secure Link Email)" ToolTip="Send the user a secure link password for password reset.  Do no use the Secure Link functionality with 'Manual' password reset."
                                            Width="300px" />
                                    </div>
                                </div>
                            </div>
                            <div class="row">&nbsp;</div>
                            <div class="roundShadowContainer" style="visibility: visible; height: 270px;">
                                <div class="row">
                                    <div class="col-sm-4">
                                        <label for="PasswordTextbox" class="labelweak">New Password:</label>
                                        <br />
                                        <asp:TextBox ID="PasswordTextbox" runat="server" TextMode="Password" Width="300px" CssClass="form-control"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="PasswordRequiredValidator" runat="server" ControlToValidate="PasswordTextbox" Display="Dynamic" ErrorMessage="Required" ValidationGroup="changepassword"></asp:RequiredFieldValidator>
                                    </div>
                                    <div class="row">&nbsp;</div>
                                    <div class="col-sm-4">
                                        <asp:Label ID="Suggested" runat="server" EnableViewState="False" Enabled="False" Width="300px" CssClass="form-control"></asp:Label>
                                    </div>
                                </div>
                                <div class="row">&nbsp;</div>
                                <div class="row">
                                    <div class="col-sm-4">
                                        <label for="PasswordConfirmTextbox" class="labelweak">Confirm New Password:</label>
                                        <br />
                                        <asp:TextBox ID="PasswordConfirmTextbox" runat="server" TextMode="Password" Width="300px" CssClass="form-control"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="PasswordConfirmRequiredValidator" runat="server" ControlToValidate="PasswordConfirmTextbox" Display="Dynamic" ErrorMessage="Required" ValidationGroup="changepassword"></asp:RequiredFieldValidator>
                                        <br />
                                        <asp:CompareValidator ID="PasswordConfirmCompareValidator" runat="server" ControlToCompare="PasswordTextBox" ControlToValidate="PasswordConfirmTextbox" Display="Dynamic" ErrorMessage="NEW password must match CONFIRM password." ValidationGroup="changepassword"></asp:CompareValidator>
                                    </div>
                                </div>
                                <br />
                                <div class="row">
                                    <div class="col-sm-4">
                                        <asp:Button ID="ChangePasswordButton" CssClass="btn btn-primary" runat="server" OnClick="ChangePassword_OnClick" Text="Manual Password Reset" ValidationGroup="changepassword" Width="300px" ToolTip="Manual password reset will not send an email to the user, this functionality should be used when the user is not recieving a secure link email." />
                                    </div>
                                </div>
                            </div>
                            <div class="row">&nbsp;</div>
                            <div>
                                <asp:Label ID="Msg" runat="server" ForeColor="Maroon" Font-Bold="true" Font-Size="14px"></asp:Label>
                            </div>
                            <div class="roundShadowContainer" style="visibility: hidden">
                                <div class="page-header engravedHeader">
                                    <h2>Change Password Q & A</h2>
                                </div>
                                <table class="table table-hover">
                                    <tbody>
                                        <tr>
                                            <td>
                                                <label for="PasswordTextbox" class="labelweak">Password:</label></td>
                                            <td>
                                                <asp:TextBox ID="qaCurrentPassword" runat="server" TextMode="Password" Width="300px" CssClass="form-control"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="qaCurrentPassword" ErrorMessage="Required" ValidationGroup="changePasswordQA"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <label for="PasswordConfirmTextbox" class="labelweak">New Password Security Question:</label></td>
                                            <td>
                                                <asp:TextBox ID="qaNewQuestion" runat="server" MaxLength="256" Width="300px" CssClass="form-control"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="QuestionRequiredValidator" runat="server" ControlToValidate="qaNewQuestion" ErrorMessage="Required" ValidationGroup="changePasswordQA"></asp:RequiredFieldValidator>
                                            </td>
                                            <tr>
                                                <td>
                                                    <label for="PasswordConfirmTextbox" class="labelweak">New Passsword Security Answer:</label></td>
                                                <td>
                                                    <asp:TextBox ID="qaNewAnswer" runat="server" MaxLength="128" Width="300px" CssClass="form-control"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="AnswerRequiredValidator" runat="server" ControlToValidate="qaNewAnswer" ErrorMessage="Required" ValidationGroup="changePasswordQA"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                    </tbody>
                                </table>
                                <asp:Button ID="ChangePasswordQuestionButton" CssClass="btn btn-primary" runat="server" OnClick="ChangePasswordQuestion_OnClick" Text="Change Q. and A." ValidationGroup="changePasswordQA" />
                            </div>
                        </div>
                    </ContentTemplate>
                </cc1:TabPanel>
            </cc1:TabContainer>
            <br />
            <%-- ajax update panel end --%>
        </ContentTemplate>
    </asp:UpdatePanel>

</div>

<%-- jquery js --%>
<uc4:jquery ID="jquery1" runat="server" />
