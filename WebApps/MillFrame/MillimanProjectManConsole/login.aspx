<%@ Page Title="Login" Language="C#" MasterPageFile="~/themes/defaults/default.master" AutoEventWireup="True" CodeBehind="login.aspx.cs" Inherits="login" EnableViewState="false" %>

<%@ Register Src="controls/login-with-captcha.ascx" TagName="LoginWithCaptcha" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <meta name="robots" content="NOINDEX, NOFOLLOW" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script type="text/javascript">
        //***************** Allowed loginWithCaptcha ******************************//
        //if there are values for loginWithCaptcha then display it
        var loginWithCaptcha = "<%= System.Configuration.ConfigurationManager.AppSettings["loginWithCaptcha"].ToString() %>"
        debugger;
        if (loginWithCaptcha.length > 1) {
            $('#divloginWithCaptcha').show();
        }
        else {
            $('#divloginWithCaptcha').hide();
        }
    </script>
    <div id="divloginWithCaptcha">
        <uc1:LoginWithCaptcha ID="LoginWithCaptcha1" runat="server" />
    </div>
</asp:Content>



