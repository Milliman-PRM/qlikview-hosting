<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Time.aspx.cs" Inherits="CLSMedicareReimbursement.Time" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
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
            <span class="title">Message:</span><asp:Label ID="lblMessage" runat="server"></asp:Label>
            <br />
            <span class="title">Disck Space:</span><asp:Label ID="lblDiskSpace" runat="server"></asp:Label>
            <br />
            <span class="title">Memory:</span><asp:Label ID="lblMemory" runat="server"></asp:Label>
            <br />
            <span class="title">System Health:</span><asp:Label ID="lblSystemHealth" runat="server"></asp:Label>
        </fieldset>        
        <fieldset>
            <legend>Server Info</legend>
            <span class="title">Web Service:</span><asp:Label ID="lblWebServiceInfo" runat="server"></asp:Label>
            <br />
            <span class="title">Database:</span><asp:Label ID="lblDbInfo" runat="server"></asp:Label>
            <br />
            <span class="title">Schema:</span><asp:Label ID="lblSchemaInfo" runat="server"></asp:Label>
        </fieldset>
        <fieldset>
            <legend>Your Browser</legend>
            <span class="title">Browser:</span><asp:Label ID="lblBrowserStuff" runat="server"></asp:Label>
            <br />
            <span class="title">Is Mobile:</span><asp:Label ID="lblIsMobile" runat="server"></asp:Label>
            <br />
            <span class="title">Screen:</span><asp:Label ID="lblScreen" runat="server"></asp:Label>
            <br />
            <span class="title">SupportS:</span><asp:Label ID="lblSupports" runat="server"></asp:Label>
            <br />
            <span class="title">Platform:</span><asp:Label ID="lblPlatform" runat="server"></asp:Label>
            <br />
            <span class="title">Needs Special VS:</span><asp:Label ID="lblNeedsSpecialVS" runat="server"></asp:Label>
        </fieldset>
    </form>
</body>
</html>
