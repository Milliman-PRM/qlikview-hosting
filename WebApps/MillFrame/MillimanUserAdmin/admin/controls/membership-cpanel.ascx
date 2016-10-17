<%@ Control Language="C#" AutoEventWireup="true" CodeFile="membership-cpanel.ascx.cs" Inherits="admin_controls_membership_cpanel" %>
<%--<div class="adminHelp">
    <br />To get started in this category, please select a destination below. 
    You can also access these categories in the navigation menu above.
</div>--%>
<style>
    .containerWrap {
        width: 1267px;
        background: none;
    }
    #divAccoutns a:hover {
        color: blue;
        text-decoration: underline;
        cursor: pointer;
    }
    .page-header {
        padding-bottom: 10px;
        margin: 5px 0 10px;
        border-bottom: 1px solid #eee;
    }
    #divNavs
    {padding:6px;width: 1267px;}
    .nav
    {
        width: 1267px;
    }
</style>
<div class="containerWrap center-block">
    <div id="divAccoutns" class="roundShadowContainer">
        <div class="page-header engravedHeader">
            <h2>User Accounts  <small>Select Setting</small></h2>
        </div>
        <div class="space"></div>
        <div id="divNavs">
            <ul class="nav nav-pills">
                <li class="active">
                    <a href="users-a-z.aspx" class="icon_add_user" title="View ALL membership users with A-Z navigation bar">
                        <i class="glyphicon glyphicon-sort-by-alphabet"></i>&nbsp;A-Z</a></li>
                <li>
                    <a href="bulk-add-user.aspx" class="icon_addUser" title="Create NEW membership USERS with Groups(s) and Profile">
                        <i class="glyphicon glyphicon-plus"></i><i class="glyphicon glyphicon-user"></i>&nbsp;Add New</a></li>
                <li>
                    <a href="users-by-role.aspx" class="icon_group" title="View membership user accounts BY GROUP">
                        <i class="glyphicon glyphicon-user"></i>&nbsp;By Group</a></li>
                <li>
                    <a href="dashboard.aspx" class="icon_dashboard" title="View membership account STATISTICS">
                        <i class="glyphicon glyphicon-dashboard"></i>&nbsp;Dashboard</a></li>
                <li>
                    <a href="email-broadcast.aspx" class="icon_email" title="Send BULK Email to registered users">
                        <i class="glyphicon glyphicon-envelope"></i>&nbsp;E-mail</a></li>
                <li>
                    <a href="locked-users.aspx" class="icon_lock" title="View all LOCKED OUT membership user accounts">
                        <i class="glyphicon glyphicon-lock"></i>&nbsp;Locked</a></li>
                <li>
                    <a href="online-users.aspx" class="icon_login" title="View all currently LOGGED-IN membership user accounts">
                        <i class="glyphicon glyphicon-log-in"></i>&nbsp;Logged In</a></li>
                <li>
                    <a href="access-reports.aspx" class="icon_file" title="Display reports that may be accessed per User or Group">
                        <i class="glyphicon glyphicon-duplicate"></i>&nbsp;Reports</a></li>
                <li>
                    <a href="roles.aspx" class="icon_group" title="Manage membership GROUPS">
                        <i class="glyphicon glyphicon-user"></i>&nbsp;Groups</a></li>
                <li>
                    <a href="access-rules.aspx" class="icon_rules" title="Manage DIRECTORY ACCESS rules for your site">
                        <i class="glyphicon  glyphicon-list-alt"></i>&nbsp;Rules</a></li>
                <li>
                    <a href="search-user.aspx" class="icon_search" title="SEARCH membership users account by keyword">
                        <i class="glyphicon glyphicon-search"></i>&nbsp;Search</a></li>
                <li>
                    <a href="unapproved-users.aspx" class="icon_block_user" title="View all Suspended membership user accounts">
                        <i class="glyphicon glyphicon-remove"></i>&nbsp;Suspended</a></li>
                <li>
                    <a href="supergroups.aspx" class="icon_superUser" title="Manager super groups and super group access">
                        <i class="glyphicon-asterisk"></i><i class="glyphicon glyphicon-user"></i>&nbsp;Super Groups</a></li>
            </ul>
        </div>
    </div>
</div>
