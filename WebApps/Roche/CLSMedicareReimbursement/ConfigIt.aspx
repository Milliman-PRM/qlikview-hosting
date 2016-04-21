<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConfigIt.aspx.cs" Inherits="CLSMedicareReimbursement.ConfigIt" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
        <form id="form1" runat="server">
    <table>  
        <tr><td>ConfigFile</td><td><asp:Label id="lblConfigFile" runat="server"></asp:Label></td></tr>      
        <tr><td>ConfigurationUserLevelconfigPath</td><td><asp:Label id="lblConfigurationUserLevelconfigPath" runat="server"></asp:Label></td></tr>
        <tr><td>ConfigurationGetExecutingAssembly</td><td><asp:Label id="lblConfigurationGetExecutingAssembly" runat="server"></asp:Label></td></tr>
        <tr><td colspan="2"><b>The custom EnvirmentSettings</b></td></tr>
        <tr><td>Enviroment</td><td><asp:Label id="lbEnvior" runat="server"></asp:Label></td></tr>
        <tr><td>DB connection</td><td><asp:Label ID="lbEnvirSQL" runat="server"></asp:Label></td></tr>
        <tr><td>SMTP Server</td><td><asp:Label ID="lbEnvirSMTP" runat="server"></asp:Label></td></tr>
        <tr><td colspan="2">&nbsp;</td></tr>
        <tr><td colspan="2"><b>From the WebConfigurationManager</b></td></tr>
        <tr><td>DB Connection</td><td><asp:Label ID="lbWebConfigManSQL" runat="server"></asp:Label></td></tr>
        <tr><td colspan="2">&nbsp;</td></tr>
        <tr><td colspan="2"><b>From getting the web.config file directly</b></td></tr>
        <tr><td>DB Connection</td><td><asp:Label id="lbWebConfigDirectSQL" runat="server"></asp:Label></td></tr>
        <tr><td>SMTP Server</td><td><asp:Label ID="lbWebConfigDirectSMTP" runat="server"></asp:Label></td></tr>
        <tr><td colspan="2">&nbsp;</td></tr>
        <tr><td colspan="2"><b>From the ConfigurationManager (not suggested since this is a web app)</b></td></tr>
        <tr><td>DB Connection</td><td><asp:Label ID="lbConfigManSQL" runat="server"></asp:Label></td></tr>
        <tr><td>info</td><td><asp:Label ID="lblInfo" runat="server"></asp:Label></td></tr>
        <tr><td>info</td><td><asp:Label ID="lblInfo2" runat="server"></asp:Label></td></tr>
    </table>
            

    </form>
</body>
</html>
