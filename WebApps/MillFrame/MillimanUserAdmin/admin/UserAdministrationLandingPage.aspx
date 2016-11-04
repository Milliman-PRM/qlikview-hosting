<%@ Page Language="C#" MasterPageFile="~/admin/themes/default/default.master" AutoEventWireup="true" CodeFile="UserAdministrationLandingPage.aspx.cs" Inherits="admin_UserAdministrationLandingPage" %>


<%@ Register Src="controls/users-a-to-z.ascx" TagName="usersAZ" TagPrefix="uc1" %>
<%@ Register Src="controls/users-by-role.ascx" TagName="usersbyrole" TagPrefix="uc2" %>
<%@ Register Src="controls/email-broadcast.ascx" TagName="emailBroadcast" TagPrefix="uc3" %>
<%@ Register Src="controls/users-locked-out.ascx" TagName="userslockedout" TagPrefix="uc4" %>
<%@ Register Src="controls/users-online.ascx" TagName="usersonline" TagPrefix="uc5" %>
<%@ Register Src="controls/search-membership-users.ascx" TagName="searchmembership" TagPrefix="uc6" %>
<%@ Register Src="controls/unapproved-users.ascx" TagName="unapproved" TagPrefix="uc7" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <style type="text/css">
        #GB_window .header .close {
            opacity: 1.5 !important;
            padding: 4px;
            width: 39px !important;
            height: 36px !important;
        }

            #GB_window .header .close:hover,
            .close:focus {
                opacity: 1.5 !important;
                padding: 4px;
                width: 39px !important;
                height: 36px !important;
            }

            #GB_window .header .close img {
                opacity: 1.5 !important;
                padding: 4px;
                width: 39px !important;
                height: 36px !important;
            }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div id="divusersAZ" runat="server" visible="false">
        <uc1:usersAZ ID="usersAZ" runat="server" />
    </div>
    <div id="divusersbyrole" runat="server" visible="false">
        <uc2:usersbyrole ID="usersbyrole" runat="server" />
    </div>
    <div id="divemailBroadcast" runat="server" visible="false">
        <uc3:emailBroadcast ID="emailBroadcast" runat="server" />
    </div>
    <div id="divuserslockedout" runat="server" visible="false">
        <uc4:userslockedout ID="userslockedout" runat="server" />
    </div>
    <div id="divuserOnline" runat="server" visible="false">
        <uc5:usersonline ID="userOnline" runat="server" />
    </div>
    <div id="divsearchmembership" runat="server" visible="false">
        <uc6:searchmembership ID="searchmembership" runat="server" />
    </div>
    <div id="divunapproved" runat="server" visible="false">
        <uc7:unapproved ID="unapproved" runat="server" />
    </div>
</asp:Content>
