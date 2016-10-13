<%@ Control Language="C#" AutoEventWireup="true" CodeFile="login-with-captcha.ascx.cs" Inherits="controls_login_with_captcha" %>
<%@ Register TagPrefix="cc1" Namespace="WebControlCaptcha" Assembly="WebControlCaptcha" %>
<%-- LOGIN USER CONTROL WITH CAPTCHA --%>
<div class="liWrap" style="font-size: 10pt; width: 200px;">
    <a name="login" id="login" style="display: block; height: 0px; width: 0px; border: 0px;"></a>
    <div class="liTitle" style="font-size: 14pt; text-align: center">
        <img src="images/PRMLogo_height80.png" style="width: 99%" />
        User Administration
    </div>
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True" ValidationGroup="Login1" EnableClientScript="False" />
    <%-- success label --%>
    <div class="LiMessage" style="text-align: center">
        <asp:HyperLink ID="lblFailureText" runat="server" Width="99%" Visible="false" EnableViewState="false"></asp:HyperLink>
    </div>
    <div class="clearBoth"></div>
    <asp:Literal ID="Msg" runat="server" Visible="false"></asp:Literal>
    <asp:LoginView ID="loginBox" runat="server">
        <%-- <LoggedInTemplate>
        </LoggedInTemplate>--%>
        <AnonymousTemplate>
            <%-- <div class="loginIcon">
            </div>--%>
            <asp:Login ID="Login1" runat="server" OnAuthenticate="Login1_Authenticate" OnLoginError="Login1_LoginError" VisibleWhenLoggedIn="False" OnLoggingIn="Login1_LoggingIn" OnLoggedIn="Login1_LoggedIn">
                <LayoutTemplate>
                    <asp:Panel ID="pnlLogin" runat="server" DefaultButton="LoginButton">
                        <div class="clearBoth2">
                        </div>
                        <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">User Name:*</asp:Label>
                        <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName" ErrorMessage="User Name is required." ToolTip="User Name is required." ValidationGroup="Login1" Display="Dynamic" EnableClientScript="False">*</asp:RequiredFieldValidator>
                        <asp:TextBox ID="UserName" runat="server" TabIndex="1" ToolTip="enter your user name" MaxLength="50" Width="99%"></asp:TextBox>
                        <div class="clearBoth2">
                        </div>
                        <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">Password:*</asp:Label>
                        <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password" ErrorMessage="Password is required." ToolTip="Password is required." ValidationGroup="Login1" Display="Dynamic" EnableClientScript="False">*</asp:RequiredFieldValidator>
                        <asp:TextBox ID="Password" runat="server" TabIndex="2" TextMode="Password" ToolTip="enter your password" MaxLength="50" Width="99%"></asp:TextBox>
                        <div class="clearBoth2">
                        </div>
                        <div class="hr">
                            <center><b>SECURITY CODE</b></center>
                        </div>
                        <div id="divCaptchaControl" title="enter the code shown on the image.">
                            <cc1:CaptchaControl ID="CAPTCHA" runat="server" LayoutStyle="Vertical"
                                ShowSubmitButton="False" TabIndex="3" CaptchaFontWarping="Extreme"
                                CssClass="captcha" ToolTip="enter the code shown above" />
                        </div>
                        <div class="clearBoth2">
                        </div>
                        <asp:Button ID="LoginButton" runat="server" CommandName="Login" TabIndex="4" Text="Log In" ValidationGroup="Login1" Font-Size="10pt" Font-Bold="True" Width="99%" />
                        <div class="clearBoth2">
                        </div>
                    </asp:Panel>
                </LayoutTemplate>
            </asp:Login>
        </AnonymousTemplate>
    </asp:LoginView>
        <br />
    <br />
     <div id="divSecCodeInfo" runat="server" visible="false" style="background:#e3dede; border:2px dashed #808080; font: normal 15px ariel; color:red; font-style:normal;font-weight:bold;  text-align: center">
        <center>Your web setting allows you to login without entering the Security Code!</center>
    </div>
</div>
