<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Diagnose.aspx.cs" Inherits="MillimanSupport.Diagnose" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <style>
#StatusTable
{
	font-family: "Lucida Sans Unicode", "Lucida Grande", Sans-Serif;
	font-size: 12px;
	/*margin: 45px;*/
	width: 600px;
	text-align: left;
	border-collapse: collapse;
	border: 1px solid lightgray;
}
#StatusTable th
{
	padding: 15px 10px 10px 10px;
	font-weight: normal;
	font-size: 14px;
	color: #039;  /*text forecolor*/
}
#StatusTable tbody
{
	background: white;  /* #F8F9FF  table background*/
}
#StatusTable td
{
	padding: 10px;
	color: #669;
	border-top: 1px dashed #fff;
}
#StatusTable tbody tr:hover td
{
	color: #339;
	background: #d0dafd;
}
    </style>
</head>
<body style="background-color:white;background-image:url(images/watermark.png);background-repeat:repeat">
    <form id="form1" runat="server">
    <div>
        <table style="margin-left:auto;margin-right:auto;border:1px solid lightgray;">
            <tr>
                <td style="height:24px;">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td style="text-align:center">
                    <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/PRMLogo_height80.png" />
                </td>
            </tr>
            <tr>
                <td style="float:right">
                    <asp:HyperLink ID="LaunchEmail1" runat="server">
                        <asp:Image ID="Image2" runat="server" ImageUrl="~/Images/mail-arrow-right-icon.png" ToolTip="Send test results to Milliman Support" BorderStyle="None" />
                    </asp:HyperLink>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Table ID="StatusTable" runat="server">
    	            <asp:TableHeaderRow>
        	            <asp:tableheadercell Width="200">Test Action</asp:tableheadercell>
                        <asp:tableheadercell Width="50">Results</asp:tableheadercell>
                        <asp:tableheadercell Width="500">Notes</asp:tableheadercell>
                    </asp:TableHeaderRow> 
                    </asp:Table>
                </td>
            </tr>
            <tr>
                <td style="float:right"><asp:HyperLink ID="LaunchEmail2" runat="server"><asp:Image ID="Image3" runat="server" ImageUrl="~/Images/mail-arrow-right-icon.png" ToolTip="Send test results to Milliman Support" BorderStyle="None" /></asp:HyperLink></td>
            </tr>
        </table>
    </div>
    </form>

    <script type="text/javascript">
        function ping(host, port, pong) {

            var started = new Date().getTime();
            //alert("ping");
            var http = new XMLHttpRequest();
            //alert("hi");
            //alert(  http.toString() );
            http.open("GET", host + ":" + port, /*async*/true);
            //alert("get");
            http.onreadystatechange = function () {
                //alert(http.readyState);
                if (http.readyState == 4) {
                    var ended = new Date().getTime();

                    var milliseconds = ended - started;

                    if (pong != null) {
                        pong(milliseconds);
                    }
                }
            };
            try {
                http.send(null);
            } catch (exception) {
                //alert('kaboom');
            }

        }

        function SetPingValue(Milliseconds) {
            var Ping = document.getElementById("ping");
            var Email1 = document.getElementById("LaunchEmail1");
            var Email2 = document.getElementById("LaunchEmail2");
            //alert("setpingvalue");
            if (Ping) {
                if (Milliseconds < 1000) {
                    Ping.innerHTML = "Less than 1 second.";
                }
                else {
                    var Seconds = Math.floor(Milliseconds / 1000);
                    Ping.innerHTML = "Less than " + Seconds + " seconds.";

                }
                Ping.title = Milliseconds + " ms";
            }

            if (Ping && Email1 && Email2) {
                var url = Email1.href;
                url = url.replace("---", Milliseconds);
                Email1.href = url;
                Email2.href = url;
            }
        }

        ping("https://hcintel.milliman.com/MillimanSupport/time.aspx", 443, SetPingValue);


    </script>
</body>
</html>

