<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="CLSConfiguration._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <script src="scripts/jquery-1.12.1.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        if (typeof jQuery == 'undefined') {
            alert("jQuery could not be loaded.....");
        }
    </script>
    <style type="text/css">
        body{
            background-image:url("images/watermark.png");
            font:normal 15px Times New Roman;
        }
        
        .roundedcorner
        {
            border-radius: 5px; 
            -moz-border-radius: 5px; 
            -webkit-border-radius: 5px; 
            border: 1px solid lightgray;
            font-style:normal;
        }
    </style>
    <title>Lab Systems Handbook Configuration Editor</title>
</head>

<body onload="Ready();"  >
    <form id="form1" runat="server">
        <div id = "Content" style = "border:1px solid lightgray;width:800px;height:600px;visibility:hidden">
            <table style="width:100%;height:100%">
                <tr>
                  <td style="vertical-align: middle; text-align: center">
                        <fieldset style="height:40px">
                            <legend>Current Staging Schema</legend>
                            <asp:Label ID="StagingSchema" runat="server" Text="rmrrdb_2016022_xy" Font-Bold="True"></asp:Label>
                         </fieldset>
                     </td>
                    <td style="vertical-align: middle; text-align: center">
                        <fieldset style="height:40px">
                            <legend>Current Production Schema</legend>
                            <asp:Label ID="ProductionSchema" runat="server" Text="rmrrdb_2016022_ab" Font-Bold="True"></asp:Label>
                         </fieldset>
                     </td>
                 </tr>
                <tr>
                    <td  colspan="2">
                      <fieldset style="height:525px">
                            <legend>Configuration Selection</legend>

                            <table>
                                <tr>
                                    <td style="text-align:center; background-color:#EAEAEA; opacity:0.65">Available Schemas</td>
                                    <td></td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td rowspan="4">
                                        <asp:ListBox ID="AllSchemas" runat="server" CssClass="roundedcorner" style="width:480px;height:475px"></asp:ListBox>
                                    </td>
                                    <td></td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="ActivateStaging" CssClass="roundedcorner" runat="server" Text="Set Active on 'STAGING' system" Width="250px" ToolTip="Make the selected schema active on the staging system" OnClick="ActivateStaging_Click" />
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="TestStaging" runat="server" ImageUrl="~/images/Tests-icon.png" ToolTip="Launch 'Lab Systems Handbook' on staging for verification/validation"  />
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td>                                        
                                        <asp:Button ID="ActivateProduction" CssClass="roundedcorner" runat="server" Text="Set Active on 'PRODUCTION' system" Width="250px" ToolTip="Make the selected schema active on the production system" OnClientClick="return ConfirmMakeLive();" OnClick="ActivateProduction_Click" />
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="TestProduction" runat="server" ImageUrl="~/images/Tests-icon.png" ToolTip="Launch 'Lab Systems Handbook' on production for verification/validation" />
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td style="font-style: italic; text-align: center;"><asp:Label ID="UserID" runat="server" Text=""></asp:Label></td>
                                    <td></td>
                                </tr>
                            </table>

                      </fieldset>
                    </td>
                </tr>
            </table>
        </div>
    </form>


    <script type="text/javascript">

        function ConfirmMakeLive()
        {
            return confirm("This action will make the selected schema live on the production system and will be viewable by external users.\n\n  Do you wish to continue?");
        }

        function Ready()
        {
            CenterContent();
        }
       function CenterContent() {
            var top = Math.max($(window).height() / 2 - $("#Content")[0].offsetHeight / 2, 0);
            var left = Math.max($(window).width() / 2 - $("#Content")[0].offsetWidth / 2, 0);
            $("#Content").css('top', top + "px");
            $("#Content").css('left', left + "px");
            $("#Content").css('position', 'fixed');
            var Con = document.getElementById('Content');
            if (Con)
                Con.style.visibility = 'visible';
       };
       $(window).resize(function () { CenterContent(); });
    </script>
</body>
</html>
