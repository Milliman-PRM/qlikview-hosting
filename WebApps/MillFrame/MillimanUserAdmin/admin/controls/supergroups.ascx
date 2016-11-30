<%@ Control Language="C#" AutoEventWireup="true" CodeFile="supergroups.ascx.cs" Inherits="admin_controls_supergroups" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="js-include1.ascx" TagName="js" TagPrefix="uc1" %>
<%@ Register Src="js-include2.ascx" TagName="js" TagPrefix="uc2" %>
<%@ Register Src="js-include3.ascx" TagName="js" TagPrefix="uc3" %>
<%@ Register Src="~/js/js/jquery.ascx" TagName="jquery" TagPrefix="uc4" %>

<div class="gvBanner">
    <span class="gvBannerThemes">
        <asp:Image ID="Image2" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/images/decoy-icon-36px.png" /></span> Super Groups
</div>

<div class="cuwWrap" id="container" style="position: absolute; top: 150px; left: 20px; width: 1024px; height: 700px; z-index: -1;">

    <asp:Label Style="position: absolute; top: 43px; left: 15px; height: 19px; width: 150px" ID="Label1" runat="server" Text="Label">Available Super Groups</asp:Label>

    <asp:ListBox Style="position: absolute; top: 63px; left: 11px; height: 531px; width: 264px" ID="SuperGroups" runat="server" OnSelectedIndexChanged="SuperGroups_SelectedIndexChanged" AutoPostBack="True"></asp:ListBox>

    <asp:TextBox Style="position: absolute; top: 607px; left: 5px; height: 20px; width: 108px; right: 903px;" ID="NewSuperGroupName" runat="server" Rows="1"></asp:TextBox>

    <asp:Button Style="position: absolute; top: 611px; left: 205px; height: 21px; width: 66px" ID="DeleteSuperGroup" runat="server" Text="Delete" OnClick="DeleteSuperGroup_Click" OnClientClick="return confirm('Are you certain you wish to delete the super group?');" />

    <asp:Button Style="position: absolute; top: 610px; left: 130px; height: 21px; width: 66px" ID="AddNewSuperGroup" runat="server" Text="Create" OnClick="AddNewSuperGroup_Click" />

    <asp:TextBox Style="position: absolute; top: 62px; left: 331px; height: 60px; width: 259px;" ID="Description" runat="server" Rows="1" TextMode="MultiLine"></asp:TextBox>

    <asp:Label Style="position: absolute; top: 45px; left: 333px; height: 19px; width: 150px" ID="SelectedSuperGroup0" runat="server" Text="Description"></asp:Label>

    <asp:CheckBox ID="UseCommaDelimited" runat="server" style="z-index: 1; left: 345px; top: 172px; position: absolute" Text="Use comma as multiple email delimiter"  ToolTip="Check the box to use comma as multiple email delimiter and UnCheck the box to use semi colon as multiple email delimiter " />
    <asp:CheckBox ID="SmartLinkOn" runat="server" style="z-index: 1; left: 345px; top: 146px; position: absolute" Text="Use Smart Link for new accounts" />

    <asp:Label Style="position: absolute; top: 200px; left: 327px; height: 19px; width: 150px" ID="SuperConsistsOf" runat="server" Text="Super group consists of"></asp:Label>
    <asp:ListBox ID="GroupsInSuper" runat="server" Style="z-index: 1; left: 322px; top: 218px; position: absolute; height: 105px; width: 285px" AutoPostBack="True" OnPreRender="Sorted_PreRender"></asp:ListBox>
    <asp:ImageButton Style="position: absolute; top: 265px; left: 640px; height: 24px; width: 24px" ID="RemoveGroup" runat="server" Text="&gt;&gt;&gt;" OnClick="RemoveGroup_Click" ImageUrl="~/images/next.png" />

    <asp:ImageButton ID="AddToSuperGroup" runat="server" Style="z-index: 1; left: 640px; top: 227px; position: absolute; width: 24px; height: 24px;" Text="&lt;&lt;&lt;" OnClick="AddToSuperGroup_Click" ImageUrl="~/images/back.png" />
    <asp:ListBox ID="AllGroups" runat="server" Style="z-index: 1; left: 688px; top: 218px; position: absolute; height: 101px; width: 312px" AutoPostBack="True" OnPreRender="Sorted_PreRender"></asp:ListBox>
    <asp:Label Style="position: absolute; top: 199px; left: 693px; height: 15px; width: 191px" ID="GroupNote" runat="server" Text="All groups in PRM system"></asp:Label>

    <asp:Label Style="position: absolute; top: 349px; left: 332px; height: 16px; width: 150px" ID="Label2" runat="server" Text="Client Admin Users"></asp:Label>
    <asp:ListBox ID="ClientAdminUsers" runat="server" Style="z-index: 1; left: 322px; top: 368px; position: absolute; height: 105px; width: 285px; right: 417px;" AutoPostBack="True" OnPreRender="Sorted_PreRender" Enabled="False" ToolTip="This functionality is disabled until the release for Millframe 4.5"></asp:ListBox>
    <asp:ImageButton Style="position: absolute; top: 421px; left: 640px; height: 24px; width: 24px" ID="RemoveAdmin" runat="server" Text="&gt;&gt;&gt;" OnClick="RemoveAdmin_Click" ImageUrl="~/images/next.png" Enabled="False" ToolTip="This functionality is disabled until the release for Millframe 4.5" />

    <asp:ImageButton ID="AddAdmin" runat="server" Style="z-index: 1; left: 640px; top: 380px; position: absolute; width: 24px; height: 24px;" Text="&lt;&lt;&lt;" OnClick="AddAdmin_Click" ImageUrl="~/images/back.png" Enabled="False" ToolTip="This functionality is disabled until the release for Millframe 4.5" />
    <asp:ListBox ID="AllAdmins" runat="server" Style="z-index: 1; left: 692px; top: 368px; position: absolute; height: 99px; width: 300px" AutoPostBack="True" OnPreRender="Sorted_PreRender" Enabled="False" ToolTip="This functionality is disabled until the release for Millframe 4.5"></asp:ListBox>
    <asp:Label Style="position: absolute; top: 350px; left: 693px; height: 15px; width: 221px" ID="Label3" runat="server" Text="All client admins with group access"></asp:Label>

    <asp:Label Style="position: absolute; top: 504px; left: 339px; height: 19px; width: 150px" ID="Label4" runat="server" Text="Client Publshing Admins"></asp:Label>
    <asp:ListBox ID="PublishingUsers" runat="server" Style="z-index: 1; left: 322px; top: 524px; position: absolute; height: 105px; width: 285px" AutoPostBack="True" OnPreRender="Sorted_PreRender"></asp:ListBox>
    <asp:ImageButton Style="position: absolute; left: 639px; height: 24px; width: 24px; top: 588px" ID="RemovePublisher" runat="server" Text="&gt;&gt;&gt;" OnClick="RemovePublisher_Click" ImageUrl="~/images/next.png" />

    <asp:ImageButton ID="AddPublisher" runat="server" Style="z-index: 1; left: 639px; top: 547px; position: absolute; width: 24px; height: 24px;" Text="&lt;&lt;&lt;" OnClick="AddPublisher_Click" ImageUrl="~/images/back.png" />
    <asp:ListBox ID="AllPublishers" runat="server" Style="z-index: 1; left: 694px; top: 530px; position: absolute; height: 97px; width: 289px" AutoPostBack="True" OnPreRender="Sorted_PreRender"></asp:ListBox>
    <asp:Label Style="position: absolute; top: 507px; left: 694px; height: 15px; width: 246px" ID="Label5" runat="server" Text="All client publishers with group access"></asp:Label>

    <asp:Button Style="position: absolute; top: 646px; left: 336px; width: 255px; right: 433px;" ID="Update" runat="server" Text="Apply Changes" OnClick="Update_Click" />

    <div style="position: absolute; border: 1px solid black; background-color: #EBEBEB; top: 40px; left: 307px; width: 317px; height: 633px; z-index: -1"></div>

    <asp:Label Style="position: absolute; top: 22px; left: 307px; height: 19px; width: 317px; text-align: center" ID="SelectedSuperGroup" runat="server" Text="Selected Super Group" Font-Bold="True"></asp:Label>

</div>


<%-- jquery js --%>
<uc4:jquery ID="jquery1" runat="server" />
<uc1:js ID="js1" runat="server" />
<uc2:js ID="js2" runat="server" />