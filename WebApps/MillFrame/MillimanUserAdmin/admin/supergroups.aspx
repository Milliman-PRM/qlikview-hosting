<%@ Page Title="Reports" Language="C#" MasterPageFile="~/admin/themes/default/default.master" AutoEventWireup="true" CodeFile="supergroups.aspx.cs" Inherits="admin_supergroups" %>

<%@ Register Src="controls/supergroups.ascx" TagName="access" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <uc1:access ID="access1" runat="server" />
</asp:Content>
