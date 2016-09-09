<%@ Control Language="C#" AutoEventWireup="true" CodeFile="create-user-with-role.ascx.cs" Inherits="admin_controls_create_user_with_role" %>
<%@ Register Src="js-include3.ascx" TagName="js" TagPrefix="uc3" %>
<%@ Register src="~/js/js/jquery.ascx" tagname="jquery" tagprefix="uc4" %>
<div class="adminHelp">
    1.) Minimum Required Password Length = 7 char.<br />2.) Minimum Required Non-Alphanumeric char = 1.<br /> 3.) Passwords are case sensitive.
</div>
<%-- gridview banner --%>
<div class="gvBanner">
  <span class="gvBannerUsers">
    <asp:Image ID="Image1" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/images/decoy-icon-36px.png" /></span> Create User With Group
</div>
<%-- create user wizard with roles --%>
<div class="cuwWrap">
  <asp:CreateUserWizard ID="RegisterUserWithRoles" runat="server" ContinueDestinationPageUrl="~/Admin/add-user.aspx" OnActiveStepChanged="RegisterUserWithRoles_ActiveStepChanged" LoginCreatedUser="False" CompleteSuccessText="The account has been successfully created." UnknownErrorMessage="The account was not created. Please try again." CreateUserButtonText="Continue - Step 2" OnCreatedUser="RegisterUserWithRoles_CreatedUser" Width="100%" OnCreatingUser="RegisterUserWithRoles_CreatingUser">
      <CreateUserButtonStyle CssClass="inputButton" />
    <TitleTextStyle CssClass="cuwTitle" />
    <WizardSteps>
      <asp:CreateUserWizardStep ID="CreateUserWizardStep1" runat="server" Title="Step 1 - Basic account details">


    <ContentTemplate>
        <table border="0" style="font-size: 100%; font-family: Verdana; padding:2px; margin:2px; width:99%">
            <tr>
                <td align="center" colspan="2" style="font-weight: bold; color: white; background-color: #5d7b9d; width:99%">
                    Create a new user account</td>
            </tr>
            <tr>
                <td align="right">
                    <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">
                        User Name:</asp:Label></td>
                <td>
                    <asp:TextBox ID="UserName" runat="server" Width="120px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName"
                        ErrorMessage="User Name is required." ToolTip="User Name is required." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td align="right">
                    <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">
                        Password:</asp:Label></td>
                <td>
                    <asp:TextBox ID="Password" runat="server" TextMode="Password" Width="120px" BackColor="LightGray" Enabled="False"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
                        ErrorMessage="Password is required." ToolTip="Password is required." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td align="right">
                    <asp:Label ID="ConfirmPasswordLabel" runat="server" AssociatedControlID="ConfirmPassword">
                        Confirm Password:</asp:Label></td>
                <td>
                    <asp:TextBox ID="ConfirmPassword" runat="server" TextMode="Password" Width="120px" BackColor="LightGray" Enabled="False"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="ConfirmPasswordRequired" runat="server" ControlToValidate="ConfirmPassword"
                        ErrorMessage="Confirm Password is required." ToolTip="Confirm Password is required."
                        ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td align="right">
                    <asp:Label ID="EmailLabel" runat="server" AssociatedControlID="Email">
                        E-mail:</asp:Label></td>
                <td>
                    <asp:TextBox ID="Email" runat="server" Width="120px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="EmailRequired" runat="server" ControlToValidate="Email"
                        ErrorMessage="E-mail is required." ToolTip="E-mail is required." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
                </td>
            </tr>

<%--            Don't ask for security questions here. let user do it when changing password--%>
<%--            <tr>
                <td align="right">
                    <asp:Label ID="QuestionLabel" runat="server" AssociatedControlID="Question">
                        Security Question:</asp:Label></td>
                <td>
                    <asp:TextBox ID="Question" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="QuestionRequired" runat="server" ControlToValidate="Question"
                        ErrorMessage="Security question is required." ToolTip="Security question is required."
                        ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr style="margin-bottom:30px">
                <td align="right">
                    <asp:Label ID="AnswerLabel" runat="server" AssociatedControlID="Answer">
                        Security Answer:</asp:Label>
                    <br /><br />
                </td>
                <td>
                    <asp:TextBox ID="Answer" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="AnswerRequired" runat="server" ControlToValidate="Answer"
                        ErrorMessage="Security answer is required." ToolTip="Security answer is required."
                        ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
                </td>
            </tr>--%>
            <tr>
                <td colspan="2" >
                     <table border="0" style="font-size: 100%; font-family: Verdana; padding:2px; margin:2px; width:99%">
                            <tr>
                                <td align="center"  style="font-weight: bold; color: white; background-color: #5d7b9d; width:50%; margin-right:5px">
                                    Access Type
                                </td>
                                 <td align="center"  style="font-weight: bold; color: white; background-color: #5d7b9d; width:50%">
                                    Data Access
                                </td>
                            </tr>
                          <tr>
                              <td >
                                 
                                   <asp:RadioButtonList id="LoginType" runat="server" AutoPostBack="True" OnSelectedIndexChanged="LoginType_SelectedIndexChanged"  >
                                       <asp:ListItem selected="true" >Covisint SSO Login</asp:ListItem>
                                       <asp:ListItem>Milliman Login</asp:ListItem>
                                   </asp:RadioButtonList>
                              </td>
                               <td >
                                    <asp:CheckBox ID="DBAccess" runat="server" Checked="False" Text="Requires data access." />
                              </td>
                          </tr>
                      </table>
                </td>
            </tr>

            <tr>
                <td align="center" colspan="2">
                    <asp:CompareValidator ID="PasswordCompare" runat="server" ControlToCompare="Password"
                        ControlToValidate="ConfirmPassword" Display="Dynamic" ErrorMessage="The Password and Confirmation Password must match."
                        ValidationGroup="CreateUserWizard1"></asp:CompareValidator>
                </td>
            </tr>
            <tr>
                <td align="center" colspan="2" style="color: red">
                    <asp:Literal ID="ErrorMessage" runat="server" EnableViewState="False"></asp:Literal>
                </td>
            </tr>
           
        </table>
    </ContentTemplate>


      </asp:CreateUserWizardStep>
      <asp:WizardStep ID="SpecifyRolesStep" runat="server" StepType="Step" Title="Step 2 -  Specify Roles" AllowReturn="False">
        <div class="checkboxList" style="width: 100px; overflow: auto;">
          <asp:CheckBoxList ID="RoleList" runat="server">
          </asp:CheckBoxList>
        </div>
      </asp:WizardStep>
      <asp:CompleteWizardStep ID="CompleteWizardStep1" runat="server">
      </asp:CompleteWizardStep>
    </WizardSteps>
      <FinishCompleteButtonStyle CssClass="inputButton" />
  </asp:CreateUserWizard>
</div>
<%-- help sidebar --%>
<div id="helpSidebarShow" class="helpSidebarShow">
    <a onclick="ShowHide(); return false;" href="#">
    H<br />
    I<br />
    N<br />
    T
    </a>
</div>
<div id="helpSidebar" class="helpSidebar" style="display: none;">
    <span class="helpSidebarClose">
        <a onclick="ShowHide(); return false;" href="#">CLOSE</a>
    </span>
    <div class="clearBoth2"></div>
    <div class="helpHintIcon"></div>
    <div>
        <asp:Repeater ID="rptHelp" runat="server" DataSourceID="xmlHelp">
            <ItemTemplate>
                <div class="helpTitle">
                    <asp:Literal ID="ltlTitle" runat="server" Text='<%#XPath("title")%>'></asp:Literal>
                </div>
                <div class="helpText">
                    <asp:Literal ID="ltlText" runat="server" Text='<%#XPath("text")%>'></asp:Literal>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <asp:XmlDataSource ID="xmlHelp" runat="server" DataFile="~/admin/help/create-user-with-role.xml"></asp:XmlDataSource>
    </div>
</div>
<%-- sidebar help js --%>
<uc3:js ID="js3" runat="server" />
<%-- jquery js --%>
<uc4:jquery ID="jquery1" runat="server" />
<script type="text/javascript">
    function EnableDisable() {
        alert('yo');
        return false;
    }
</script>