<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConfigIt.aspx.cs" Inherits="CLSMedicareReimbursement.ConfigIt" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Configs Check</title>
    <meta name="ROBOTS" content="NOINDEX, NOFOLLOW" />
    <meta name="GOOGLEBOT" content="NOINDEX" />
    <style type="text/css">
        fieldset {
            border: 1px dotted #ccc;
            padding: 5px;
            margin: 0 auto;
            width: 75%;
        }

            fieldset legend {
                margin: 7px;
            }

        .title {
            float: left;
            font-weight: bold;
            padding: 0 5px 0 0;
            width: 158px;
        }
    </style>
</head>
<body>
    <form id="form2" runat="server">
        <fieldset>
            <legend>Custom EnvirmentSettings</legend>
            <span class="title">Date:</span><asp:Label ID="lblSystemDate" runat="server"></asp:Label>
            <br />
            <span class="title">Config File:</span><asp:Label ID="lblConfigFile" runat="server"></asp:Label>
            <br />
            <span class="title">Enviroment:</span><asp:Label ID="lbEnvior" runat="server"></asp:Label>
            <br />
            <span class="title">DB connection:</span><asp:Label ID="lbEnvirSQL" runat="server"></asp:Label>
            <br />
            <span class="title">SMTP Server:</span><asp:Label ID="lbEnvirSMTP" runat="server"></asp:Label>
        </fieldset>
        <fieldset>
            <legend>Web Configuration Manager</legend>
            <span class="title">DB connection:</span><asp:Label ID="lbWebConfigManSQL" runat="server"></asp:Label>
        </fieldset>
        <fieldset>
            <legend>web.config file directly</legend>
            <span class="title">DB connection:</span><asp:Label ID="lbWebConfigDirectSQL" runat="server"></asp:Label>
            <br />
            <span class="title">SMTP Server:</span><asp:Label ID="lbWebConfigDirectSMTP" runat="server"></asp:Label>
        </fieldset>
        <fieldset>
            <legend>ConfigurationManager (not suggested since this is a web app)</legend>
            <span class="title">DB connection:</span><asp:Label ID="lbConfigManSQL" runat="server"></asp:Label>
        </fieldset>
        <fieldset>
            <legend>Values Information</legend>
            <span class="title">Config Values:</span><asp:Label ID="lblInfo" runat="server"></asp:Label>
            <br />
            <br />
            <span class="title">Config Values:</span><asp:Label ID="lblInfo2" runat="server"></asp:Label>
        </fieldset>
    </form>
</body>
</html>
