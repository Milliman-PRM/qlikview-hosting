<%@ Control Language="C#" AutoEventWireup="true" CodeFile="access-reports.ascx.cs" Inherits="admin_controls_admin_reports" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="js-include1.ascx" TagName="js" TagPrefix="uc1" %>
<%@ Register Src="js-include2.ascx" TagName="js" TagPrefix="uc2" %>
<%@ Register Src="js-include3.ascx" TagName="js" TagPrefix="uc3" %>
<%@ Register Src="~/js/js/jquery.ascx" TagName="jquery" TagPrefix="uc4" %>

<style>
    .containerWrap {
        width: 800px;
    }

    #divHeaders {
        width: 650px;
    }

    #divResults {
        width: 650px;
    }

    #divUserProfileSettingsContainer {
        width: 500px;
    }
    .RadTreeView {
        margin: 6px;
        padding: 10px;
        border-image: none;
        width: 337px;
        text-align: justify;
    }
    #divTotalRecs
    {
        margin:4px 6px 4px 4px;
    }
</style>
<%--<div class="gvBanner">
    <span class="gvBannerThemes">
        <asp:Image ID="Image2" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/images/decoy-icon-36px.png" /></span> Reports
</div>--%>
<div class="containerWrap center-block outerWrap">
    <div class="page-header engravedHeader">
        <h2>Reports</h2>
    </div>
    <div class="space"></div>
    <div id="divUserProfileSettingsContainer" class="roundShadowContainer">
        <table class="table table-hover">
            <tbody>
                <tr>
                    <td>
                        <label for="UserFirstName" class="labelweak">Report Type:</label></td>
                    <td>
                        <asp:DropDownList ID="ReportType" runat="server" AutoPostBack="True" 
                            AppendDataBoundItems="true" OnSelectedIndexChanged="ReportType_SelectedIndexChanged"
                            BackColor="Window" Font-Names="Georgia" CssClass="ddl"  Style="width: 270px;">
                            <asp:ListItem Text="User / Group" Value="UserGroup"></asp:ListItem>
                            <asp:ListItem Text="User / QVWs" Value="UserQVWS"></asp:ListItem>
                            <asp:ListItem Text="Group Contents" Value="Group"></asp:ListItem>
                            <asp:ListItem Text="QVWs / User" Value="QVWSUser"></asp:ListItem>
                            <asp:ListItem Text="QVWs / Group" Value="QVWSGroup"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label id="SelectionLabel" runat="server" for="UserFirstName" class="labelweak">Select a user:</label>
                    </td>
                    <td>
                        <asp:DropDownList ID="UserSelections" runat="server" AutoPostBack="False"
                            BackColor="Window" Font-Names="Georgia" CssClass="ddl" Style="width: 270px;">
                        </asp:DropDownList>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    <div class="space"></div>
    <div class="row">
        <div class="center-block" style="float: none; width: 168px;">
            <div class="col-md-9">
                <div class="col-md-9">
                    <asp:Button ID="Generate" runat="server" Text="Generate Report" CssClass="btn btn-primary" OnClick="Generate_Click" />
                </div>
            </div>
        </div>
    </div>
    <div class="space"></div>
    <div class="row">
        <asp:UpdatePanel ID="updPanlRecords" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
            <ContentTemplate>
                <div id="divTotalRecs">
                    <span>Total Records Found:</span> <b>
                        <asp:Label ID="lblTotalRecords" runat="server" CssClass="count"></asp:Label></b>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div class="space"></div>
    <div id="divResults" class="roundShadowContainer" runat="server" visible="false">
        <telerik:RadTreeView ID="Report" runat="server"></telerik:RadTreeView>
    </div>
</div>

<%-- jquery js --%>
<uc4:jquery ID="jquery1" runat="server" />
<uc1:js ID="js1" runat="server" />
<uc2:js ID="js2" runat="server" />
