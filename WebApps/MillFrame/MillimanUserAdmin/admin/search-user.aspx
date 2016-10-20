<%@ Page Title="Membership Search" Language="C#" MasterPageFile="~/admin/themes/default/default.master" AutoEventWireup="true" CodeFile="search-user.aspx.cs" Inherits="admin_search_user" MaintainScrollPositionOnPostback="true" %>

<%@ Register Src="controls/search-membership-users.ascx" TagName="search" TagPrefix="uc1" %>
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
    <uc1:search ID="search1" runat="server" />
</asp:Content>
