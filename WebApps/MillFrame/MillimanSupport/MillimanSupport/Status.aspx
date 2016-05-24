<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Status.aspx.cs" Inherits="MillimanSupport.Status" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <telerik:RadScriptManager runat="server" ID="RadScriptManager1" />
        <telerik:RadFormDecorator ID="QsfFromDecorator" runat="server" DecoratedControls="All" EnableRoundedCorners="false" />
        <telerik:RadAjaxPanel runat="server" ID="RadAjaxPanel1" LoadingPanelID="RadAjaxLoadingPanel1">
        <table width="100%" border="1" style="border-color:#AEBDCE" cellspacing="0" >
            <tr style="height:100px;">
                <td colspan="2" >
                    <center>
                    <div style="font-family:'Times New Roman';font-size:12pt;font-style:italic;font-weight:800;">
                        <asp:Image ID="OverviewIcon" runat="server" ImageUrl="~/Images/Sign-Select-icon64.png" style="vertical-align:middle"></asp:Image>
                        <asp:Label ID="Overview" runat="server" Text="&nbsp;The PRM server has passed the diagnotics and is functioning properly."></asp:Label>
                    </div>
                    </center>
                </td>
            </tr>
            <tr>
                <td>
                     <div style="width: 600px; height: 300px;">
                         <asp:Label ID="ReportTime" runat="server" style="font-size:8pt;width:100%" Visible="False"></asp:Label>    
               <telerik:RadHtmlChart runat="server" ID="BarChart" Width="600px" Height="300px" Transitions="true" Skin="WebBlue" >
                    <Appearance>
                         <FillStyle BackgroundColor="White"></FillStyle>
                    </Appearance>
                    <ChartTitle Text="Remote Monitor Response Times(ms)">
                         <Appearance Align="Center" BackgroundColor="White" Position="Top"></Appearance>
                    </ChartTitle>
  <%--                  <Legend>
                         <Appearance BackgroundColor="White" Position="Bottom"></Appearance>
                    </Legend>--%>
                    <PlotArea>
                         <Appearance>
                              <FillStyle BackgroundColor="White"></FillStyle>
                         </Appearance>
                         <XAxis AxisCrossingValue="0" Color="#b3b3b3" MajorTickType="Outside" MinorTickType="Outside" Reversed="false">
      <%--                        <Items>
                                   <telerik:AxisItem LabelText="2003"></telerik:AxisItem>
                                   <telerik:AxisItem LabelText="2004"></telerik:AxisItem>
                                   <telerik:AxisItem LabelText="2005"></telerik:AxisItem>
                              </Items>--%>
                              <LabelsAppearance DataFormatString="{0}" RotationAngle="0" Skip="0" Step="1"></LabelsAppearance>
                              <MajorGridLines Color="#EFEFEF" Width="1"></MajorGridLines>
                              <MinorGridLines Color="#F7F7F7" Width="1"></MinorGridLines>
                           <%--   <TitleAppearance Position="Center" RotationAngle="0" Text="Years"></TitleAppearance>--%>
                         </XAxis>

                         <YAxis AxisCrossingValue="0" Color="#b3b3b3" MajorTickSize="1" MajorTickType="Outside"
                               MinorTickSize="1" MinorTickType="Outside" MinValue="0" Reversed="false"
                              >
                              <LabelsAppearance DataFormatString="{0}" RotationAngle="0" Skip="0" Step="1"></LabelsAppearance>
                              <MajorGridLines Color="#EFEFEF" Width="1"></MajorGridLines>
                              <MinorGridLines Color="#F7F7F7" Width="1"></MinorGridLines>
                           <%--   <TitleAppearance Position="Center" RotationAngle="0" Text="Sum"></TitleAppearance>--%>
                         </YAxis>
                         <Series>
                              <telerik:BarSeries Name="ResponseSeries" Stacked="false">
                                   <LabelsAppearance DataFormatString="{0}ms" Position="Center"></LabelsAppearance>
                                  <%-- <TooltipsAppearance BackgroundColor="#c5d291" DataFormatString="{0}"></TooltipsAppearance>--%>
                             <%--      <SeriesItems>
                                        <telerik:CategorySeriesItem Y="315000"></telerik:CategorySeriesItem>
                                        <telerik:CategorySeriesItem Y="495000"></telerik:CategorySeriesItem>
                                        <telerik:CategorySeriesItem Y="690000"></telerik:CategorySeriesItem>
                                   </SeriesItems>--%>
                              </telerik:BarSeries>
                         </Series>
                    </PlotArea>
               </telerik:RadHtmlChart>
               <br />
               <asp:Label ID="LinkMessage" runat="server" ForeColor="Red" Font-Size="10"></asp:Label>
          </div>
               </td>
                <td valign="middle" align="left">
                    <div style="width: 600px; height: 300px;">
                        <center>
                        <table border="1"  cellpadding="10" cellspacing="0" style="position:relative; top:30px;border-color:#AEBDCE">
                            <tr >
                                <td style="background-image:url(css/off.gif)" colspan="5"><center><asp:Label ID="Label1" runat="server" Text="Milliman HCIntel Server Components" ToolTip="https://hcintel.milliman.com"></asp:Label></center> </td>
                            </tr>
                            <tr>
                                <td>Report Subsystem</td>
                                <td>Data Subsystem</td>
                                <td>User Subsystem</td>
                                <td>Diskspace</td>
                                <td>Memory</td>
                            </tr>
                            <tr>
                                <td><img src="Images/Column-Chart-icon64.png" alt="Report Subsystem" /></td>
                                <td><img src="Images/Home-Server-icon72.png" alt="Data Subsystem" /></td>
                                <td><img src="Images/Apps-system-users-icon72.png" alt="User Subsystem" /></td>
                                <td><img src="Images/datas-icon72.png" alt="Diskspace" /></td>
                                <td><img src="Images/Device-RAM-icon72.png" alt="Memory" /></td>
                            </tr>
                            <tr>
                                <td><asp:Image ID="reportsubsystem" runat="server" ImageUrl="~/Images/Sign-Select-icon32.png" ImageAlign="Middle" /></td>
                                <td><asp:Image ID="datasubsystem" runat="server" ImageUrl="~/Images/Sign-Select-icon32.png" ImageAlign="Middle" /></td>
                                <td><asp:Image ID="usersubsystem" runat="server" ImageUrl="~/Images/Sign-Select-icon32.png" ImageAlign="Middle" /></td>
                                <td><asp:Image ID="diskspace" runat="server" ImageUrl="~/Images/Sign-Select-icon32.png" ImageAlign="Middle" /></td>
                                <td><asp:Image ID="memory" runat="server" ImageUrl="~/Images/Sign-Select-icon32.png" ImageAlign="Middle" /></td>
                            </tr>
                        </table>
                        </center>
                     </div>
                </td>
           </tr>
        </table>
        </telerik:RadAjaxPanel>
        <telerik:RadAjaxLoadingPanel runat="server" ID="RadAjaxLoadingPanel1">
         </telerik:RadAjaxLoadingPanel>
    </form>
</body>
</html>
