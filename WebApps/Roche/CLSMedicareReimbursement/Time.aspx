<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Time.aspx.cs" Inherits="CLSMedicareReimbursement.Time" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Health Check</title>
    <meta name="ROBOTS" content="NOINDEX, NOFOLLOW" />
    <meta name="GOOGLEBOT" content="NOINDEX" />
    <style type="text/css">
        fieldset {
            border: 1px dotted #ccc;
            padding: 5px;
            margin: 0 auto;
            width: 50%;
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
    <form id="form1" runat="server">
        <fieldset>
            <legend>System Information</legend>
            <span class="title">Date:</span><asp:Label ID="lblSystemDate" runat="server"></asp:Label>
            <br />
            <span class="title">StatusCode:</span><asp:Label ID="lblStatusCode" runat="server"></asp:Label>
            <br />
            <span class="title">Disck Space:</span><asp:Label ID="lblDiskSpace" runat="server"></asp:Label>
            <br />
            <span class="title">Memory:</span><asp:Label ID="lblMemory" runat="server"></asp:Label>
            <br />
            <span class="title">System Health:</span><asp:Label ID="lblSystemHealth" runat="server"></asp:Label>
        </fieldset>        
        <fieldset>
            <legend>Server Info</legend>
            <span class="title">Database:</span><asp:Label ID="lblDbInfo" runat="server"></asp:Label>
            <br />
            <span class="title">Schema:</span><asp:Label ID="lblSchemaInfo" runat="server"></asp:Label>
        </fieldset>       
    </form>
</body>
</html>
