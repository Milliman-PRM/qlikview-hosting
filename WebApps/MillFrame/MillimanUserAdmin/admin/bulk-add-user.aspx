<%@ Page Title="Create New User Account" Language="C#" MasterPageFile="~/admin/themes/default/default.master" AutoEventWireup="true" CodeFile="bulk-add-user.aspx.cs" Inherits="bulk_admin_add_user" %>

<%@ Register src="controls/bulk-create-user-with-role.ascx" tagname="create" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
  <uc1:create ID="create1" runat="server" />
</asp:Content>

