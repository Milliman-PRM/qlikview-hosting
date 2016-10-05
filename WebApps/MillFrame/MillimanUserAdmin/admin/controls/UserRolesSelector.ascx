<%@ Control Language="C#" AutoEventWireup="true" CodeFile="UserRolesSelector.ascx.cs" Inherits="admin_controls_UserRolesSelector" %>

<script type="text/javascript">
    //Javascript Code Block
</script>
<style type="text/css">
    /*view style*/
    .layOut {
        overflow-y: scroll; 
        height:175px;
    }
    #divContainer{padding:4px;border:3px solid #eee;}
    .tv{    
        margin: 0px 0px 0px 38px;
        padding: 4px;}
</style>

<%--tvUserRoles--%>
<div id="divContainer" class="divContainerMain">
    <div class="page-header engravedHeader">
        <h2>Group Selection </h2>
    </div>
    <div class="layOut">
        <asp:TreeView ID="tvUserRoles" runat="server" CssClass="tv"
            NodeStyle-Font-Names="Consolas"
            NodeStyle-Font-Size="12px"
            ShowExpandCollapse="true"
            OnSelectedNodeChanged="OnSelectedNodeChanged"
            OnTreeNodeCheckChanged="OnTreeNodeCheckChanged">
        </asp:TreeView>
    </div>
</div>

