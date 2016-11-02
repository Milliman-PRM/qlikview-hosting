<%@ Control Language="C#" AutoEventWireup="true" CodeFile="membership-cpanel.ascx.cs" Inherits="admin_controls_membership_cpanel" %>

<style>
    .outer {
        width: 745px;
    }
    .nav {
        width: 745px;
        padding: 6px;
    }
    #divAccoutns {
        padding: 4px 4px 4px 4px;
        width: 690px;
        height: 359px;
    }
    .linkItemsContainer {
        padding: 8px 8px 8px 8px;
        margin: 8px;
    }
</style>

<div class="containerWrap center-block outer outerWrap">
    <div class="page-header engravedHeader">
        <h2>User Administration&nbsp;&nbsp;<small>User Account Setting</small></h2>
    </div>
    <div class="space"></div>
    <div class="nav">
        <div id="divAccoutns" class="roundShadowContainer center-block">
            <ul class="nav nav-pills linkItemsContainer">
                <li class="active">
                    <a href="default.aspx" title="User Account" >
                        <i class="glyphicon glyphicon-home"></i>&nbsp;User Accounts                        
                    </a>
                </li>
                <li>
                    <a href="UserAdministrationLandingPage.aspx?launchSystem=usersAZ" title="View ALL membership users with A-Z navigation bar" >
                        <i class="glyphicon glyphicon-sort-by-alphabet"></i>&nbsp;A-Z</a></li>
                <li>
                    <a href="bulk-add-user.aspx" title="Add NEW membership USERS with Groups(s) and Profile" >
                        <i class="glyphicon glyphicon-plus"></i><i class="glyphicon glyphicon-user"></i>&nbsp;Add New</a></li>
                <li>
                    <a href="UserAdministrationLandingPage.aspx?launchSystem=usersbyrole" title="View membership user accounts BY GROUP" >
                        <i class="glyphicon glyphicon-user"></i><i class="glyphicon glyphicon-user"></i>&nbsp;By Group</a></li>
                <li>
                    <a href="dashboard.aspx" title="View membership account STATISTICS" >
                        <i class="glyphicon glyphicon-dashboard"></i>&nbsp;Dashboard</a></li>
                <li>
                    <a href="UserAdministrationLandingPage.aspx?launchSystem=emailBroadcast" title="Send BULK Email to registered users" >
                        <i class="glyphicon glyphicon-envelope"></i>&nbsp;E-mail</a></li>
                <li>
                    <a href="UserAdministrationLandingPage.aspx?launchSystem=userslockedout" title="View all LOCKED OUT membership user accounts" >
                        <i class="glyphicon glyphicon-lock"></i>&nbsp;Locked</a></li>
                <li>
                    <a href="UserAdministrationLandingPage.aspx?launchSystem=userOnline" title="View all currently LOGGED-IN membership user accounts" >
                        <i class="glyphicon glyphicon-log-in"></i>&nbsp;Logged In</a></li>
                <li>
                    <a href="access-reports.aspx" title="Display reports that may be accessed per User or Group" >
                        <i class="glyphicon glyphicon-duplicate"></i>&nbsp;Reports</a></li>
                <li>
                    <a href="roles.aspx" title="Manage membership GROUPS" >
                        <i class="glyphicon glyphicon-user"></i><i class="glyphicon glyphicon-user"></i>&nbsp;Groups</a></li>
                <li>
                    <a href="access-rules.aspx" title="Manage DIRECTORY ACCESS rules for your site" >
                        <i class="glyphicon  glyphicon-list-alt"></i>&nbsp;Rules</a></li>
                <li>
                    <a href="UserAdministrationLandingPage.aspx?launchSystem=searchmembership" title="SEARCH membership users account by keyword" >
                        <i class="glyphicon glyphicon-search"></i>&nbsp;Search</a></li>
                <li>
                    <a href="UserAdministrationLandingPage.aspx?launchSystem=unapproved" title="View all Suspended membership user accounts" >
                        <i class="glyphicon glyphicon glyphicon-off"></i>&nbsp;Suspended</a></li>
                <li>
                    <a href="supergroups.aspx" title="Manager super groups and super group access" >
                        <i class="glyphicon glyphicon glyphicon-king"></i>&nbsp;Super Groups</a></li>
            </ul>
        </div>
    </div>

</div>
