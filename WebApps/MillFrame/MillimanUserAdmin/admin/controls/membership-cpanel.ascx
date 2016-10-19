<%@ Control Language="C#" AutoEventWireup="true" CodeFile="membership-cpanel.ascx.cs" Inherits="admin_controls_membership_cpanel" %>
<%--<div class="adminHelp">
    <br />To get started in this category, please select a destination below. 
    You can also access these categories in the navigation menu above.
</div>--%>

<style>
    #divAccoutns a:hover {
        color: blue;
        text-decoration: underline;
        cursor: pointer;
    }
    #divNavs {
        padding: 6px 0px 10px 12px;
    }
    .nav {
        width: 1400px;
    }
    .outer {
        width: 1420px;
    }
</style>
<div class="containerWrap center-block outer  outerWrap">
    <div class="page-header engravedHeader">
        <h2>User Administration&nbsp;&nbsp;<small>User Account Setting</small></h2>
    </div>
    <div class="space"></div>
    <div id="divAccoutns" class="roundShadowContainer">
        <div id="divNavs" class="outer">
            <ul class="nav nav-pills">
                <li class="active">
                    <a href="users-a-z.aspx" title="User Account">
                        <i class="glyphicon glyphicon-home"></i>&nbsp;User Accounts</a>
                </li>
                <li>
                    <a href="users-a-z.aspx" title="View ALL membership users with A-Z navigation bar">
                        <i class="glyphicon glyphicon-sort-by-alphabet"></i>&nbsp;A-Z</a></li>
                <li>
                    <a href="bulk-add-user.aspx" title="Add NEW membership USERS with Groups(s) and Profile">
                        <i class="glyphicon glyphicon-plus"></i><i class="glyphicon glyphicon-user"></i>&nbsp;Add New</a></li>
                <li>
                    <a href="users-by-role.aspx" title="View membership user accounts BY GROUP">
                        <i class="glyphicon glyphicon-user"></i><i class="glyphicon glyphicon-user"></i>&nbsp;By Group</a></li>
                <li>
                    <a href="dashboard.aspx" title="View membership account STATISTICS">
                        <i class="glyphicon glyphicon-dashboard"></i>&nbsp;Dashboard</a></li>
                <li>
                    <a href="email-broadcast.aspx" title="Send BULK Email to registered users">
                        <i class="glyphicon glyphicon-envelope"></i>&nbsp;E-mail</a></li>
                <li>
                    <a href="locked-users.aspx" title="View all LOCKED OUT membership user accounts">
                        <i class="glyphicon glyphicon-lock"></i>&nbsp;Locked</a></li>
                <li>
                    <a href="online-users.aspx" title="View all currently LOGGED-IN membership user accounts">
                        <i class="glyphicon glyphicon-log-in"></i>&nbsp;Logged In</a></li>
                <li>
                    <a href="access-reports.aspx" title="Display reports that may be accessed per User or Group">
                        <i class="glyphicon glyphicon-duplicate"></i>&nbsp;Reports</a></li>
                <li>
                    <a href="roles.aspx" title="Manage membership GROUPS">
                        <i class="glyphicon glyphicon-user"></i><i class="glyphicon glyphicon-user"></i>&nbsp;Groups</a></li>
                <li>
                    <a href="access-rules.aspx" title="Manage DIRECTORY ACCESS rules for your site">
                        <i class="glyphicon  glyphicon-list-alt"></i>&nbsp;Rules</a></li>
                <li>
                    <a href="search-user.aspx" title="SEARCH membership users account by keyword">
                        <i class="glyphicon glyphicon-search"></i>&nbsp;Search</a></li>
                <li>
                    <a href="unapproved-users.aspx" class="icon_block_user" title="View all Suspended membership user accounts">
                        <i class="glyphicon glyphicon glyphicon-off"></i>&nbsp;Suspended</a></li>
                <li>
                    <a href="supergroups.aspx" class="icon_superUser" title="Manager super groups and super group access">
                        <i class="glyphicon glyphicon glyphicon-king"></i>&nbsp;Super Groups</a></li>
            </ul>
        </div>
    </div>
</div>
