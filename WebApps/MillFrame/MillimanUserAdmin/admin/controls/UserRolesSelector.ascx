<%@ Control Language="C#" AutoEventWireup="true" CodeFile="UserRolesSelector.ascx.cs" Inherits="admin_controls_UserRolesSelector" %>

<script type="text/javascript">
    //Javascript Code Block
</script>
<style type="text/css">
    /*view style*/
    .layOut {
        margin: 6px 1px 1px 23px;
        padding: 4px 4px 4px 4px;
        overflow-y: scroll; height:200px;
    }
</style>

<%--tvUserRoles--%>
<div id="divContainer" class="divContainerMain">
    <div class="page-header engravedHeader">
        <h2>Group Selection </h2>
    </div>
    <div class="layOut">
        <asp:TreeView ID="tvUserRoles" runat="server"
            NodeStyle-Font-Names="Consolas"
            NodeStyle-Font-Size="12px"
            ShowExpandCollapse="true"
            OnSelectedNodeChanged="OnSelectedNodeChanged"
            OnTreeNodeCheckChanged="OnTreeNodeCheckChanged">
        </asp:TreeView>
    </div>
</div>

