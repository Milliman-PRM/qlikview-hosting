<%@ Control Language="C#" AutoEventWireup="true" CodeFile="UserRolesSelector.ascx.cs" Inherits="admin_controls_UserRolesSelector" %>

<script type="text/javascript">
    //Javascript Code Block
</script>
<style type="text/css">
    /*view style*/
</style>

<div id="divContainer" class="divContainerMain">
    <asp:TreeView ID="tvUserRoles" runat="server"
        NodeStyle-Font-Names="Consolas"
        NodeStyle-Font-Size="12px"
        ShowExpandCollapse="true"
        OnSelectedNodeChanged="OnSelectedNodeChanged"
        OnTreeNodeCheckChanged="OnTreeNodeCheckChanged">
    </asp:TreeView>
</div>

