___

##**PRM Release MillFrame 4.2.0**

#***PRM Project Management Console (PMC) Guide***

___

Confidential and Proprietary

© 2017 Milliman, Inc. All rights reserved.\
Milliman® is a trademark of Milliman, Inc.\
All other trademarks belong to their respective owners

This manual and its contents are the confidential property of Milliman, Inc. and are prepared for the exclusive use of Milliman, Inc. and its subscribing clients. Any distribution or reproduction, intentional or unintentional, of any materials contained herein without the express written permission of Milliman, Inc. is prohibited.

For additional information, please contact our technical support team by email: <prm.support@milliman.com>

___


##Introduction

Milliman PRM Analytics is a predictive analytics solution that is used to identify potentially avoidable costs in populations under financial risk.

This user guide is designed to give users an understanding of the latest features of the PRM Project Management Console (PMC) and step by step instructions for using this application.

**In order to access the PRM Project Management Console, administrative rights must be granted for your account on the PRM production server. Independent of the server hosting the PMC, credentials and roles (for administrators only) are synchronized between the server hosting the PMC and the PRM production server.**


##Technical Support

If you are having any technical difficulty with the product, please contact our technical support team by email: <prm.support@milliman.com>

*Note: For the purpose of this user guide, all data has been de-identified to protect patient’s personal health information (PHI) in accordance with the Health Insurance Portability and Accountability Act of 1996 (HIPAA).*


##PRM Project Management Console


###Overview

The Project Management Console - PMC allows PRM operators to update report content on the PRM production server. This functionality includes, importing “new” reports, updating existing reports, reducing report content per user and providing auditing/validation reports as to the current status/state of a specific update. It is important to note the PMC is considered part of the PRM production system even though it does not reside on the PRM production server itself. Multiple instances of the PMC may be utilized from different servers all communicating with the PRM production server via web service calls. Each instance of PMC that communicates with the PRM production server must do so via SSL and have its IP address published to the PRM production server whitelist – otherwise the PRM production server will reject the communication requests. Credentials are maintained between PRM production server and each instance of the PMC to implement a basic single sign on mechanism. (Note: only accounts with the “administrative” role are support for SSO).

The PMC works in coordination with another PRM tool called the “PRM Signature Tool”. Each report (QVW) that must be updated on the PRM production system, must be “signed” using the signature tool. The signature provided on a report (QVW) dictates the specific area a report can be posted to. In the event the signature associated with a report, does not match the expected signature for the request, the operator is informed the report (QVW) cannot be posted and the request to update is abandoned. Signatures on reports are also used to determine if the report (QVW) is instrumented for report reduction and/or report reduction validation.

![](Images/PRM_Project_Management_Console_User_Guide/image1.png ""){width=100px height=auto} Preferred Browser: Firefox

The PMC is designated and tested to be used via Firefox due to its limited audience of users. The PMC functionality may work correctly in Internet Explorer, Edge and Chrome, but the preferred/designated browser for its use is Firefox.

###Definitions

**Group**: an arbitrary container used to associate users to reports (QVWs).

**QVW**: A Qlikview report document.

**Signature**: extra attributes attached to a report (QVW) that identifies a specific group the report may be associated with via the Project Management Console.

**Analytics Pipeline**: Onboarding, translation/transformation, and processing code used to calculate value-added data and bind data onto a Report (QVW) that is signed for a specific Group.

**Project**: a collection of items associated with a report (QVW) that provides extra information such as

-   Report Name ( displayed to user )

-   Report Description (displayed to user)

-   Tooltip (displayed to user)

-   Report Launch Icon (displayed to user)

-   Optional User Guide

-   Auditing Information

    -   Who updated the project last

    -   When was the project updated last

    -   Who last published to production server

    -   When was the last publication to server


###Generalized PMC Workflow


The workflows described in this section denote an overview of how the PMC is typically used and does not go into the nuances that may be presented due to how a report (QVW) is signed.


####Creation of a project can be done via

-   Manual Creation with PMC

    1.  Create a directory

    2.  Create a project in directory

    3.  Fill out the project settings items (name, description, icon….)

    4.  Populate the project with a report

    5.  Post the report to the production server

-   Importing a signed report (QVW) with PMC

    1.  Click the “Import” menu item (QV Icon on menu bar)

    2.  Browse to the new QVW (must be signed)

    3.  Acknowledge the new group creation dialog

    4.  PMC will then create all the required directories and settings

    5.  Fill out the project settings items (name, description, icon….)

    6.  Populate the project with the report

    7.  Post the report to the production server


####Updating report content

    1.  Navigate to the project in the explorer interface

    2.  Right click and select “Reload Project”

    3.  Upload new report (QVW) and optional user guide

    4.  Post to production


##Project Management Console


### PMC Login

To log into the PMC you must have administrator privileges. Provide your

-   User Name

-   Password

-   Enter the security code

> ![](Images/PRM_Project_Management_Console_User_Guide/image2.png ""){width=35% height=auto}\


### PMC User Interface – General Work Items

The main view in the PMC is setup to mimic Windows® file explorer with project files (extension HCIPRJ) visible in the interface. All other files types are hidden that might be present are hidden from view.

![](Images/PRM_Project_Management_Console_User_Guide/image3.png ""){width=100% height=auto}\

The PMC user interface is designed around standard user interface controls, standard menu items (menu bar) and context menus (right click). The functionality of each of these items is detailed in this section.

![](Images/PRM_Project_Management_Console_User_Guide/image4.png ""){width=100% height=auto}\

![](Images/PRM_Project_Management_Console_User_Guide/image5.png ""){width=100% height=auto}\


1.  Refresh – clicking refreshes the user interface. Used to ensure disk content and user interface is synchronized correctly.

2.  Create folder – allows an empty folder to be created at the root of the folder tree or sub-folders as needed. Folders are typically created to hold projects when preparing for manual creation of a project. This menu option is replicated on the context menu for the folder tree.

3.  Delete folder – deletes a folder, all content and all sub-folders. The menu option is replicated on the context menu for the folder tree.

4.  Import project – this option will allow a SIGNED QVW to be brought into the system and setups up the appropriate folder as determined by the signature of the QVW. This options is the recommend way of creating new content folders for the PRM system.

5.  User Guide – click to view this user guide.

6.  Server – displays the production server this instance of the PMC is communicating with. The server name displayed always represents a forward facing server that serves content to end-users.

7.  Path – this represents the currently selected tree item node and it’s path

8.  The currently logged in user

9.  Logout – click to gracefully exit PMC. If browser is closed and “Logout” was not clicked, the server will time out the session and automatically log the user off. (Note: the timeout value (minutes) is dictated by the configuration file for the PMC and is modified as HIPAA requirements change.)

10. Folder tree – displays the folders, sub folders and project files in a tree structure.

11. Copy – copies the folder and its contents for later pasting. *Note: this menu item is has been deprecated and will be removed from future versions of the PMC due to the possible issues of creating multiple projects with conflicting information (signature of QVW conflicts with project settings).*

12. Create Project – used to manually create a project in the current working directory. Selecting this option will present a dialog that must be completed to successfully create a valid working project.

13. Project listing – most folders will contain 1 or more project files (HCIPRJ). This view presents the user in the right hand grid with the following information per project file.

    1.  Project name as found on disk

    2.  Size of project file in bytes

    3.  Last date/time the local project was saved

    4.  An icon that represents if the production server version of this project is EXACTLY the same as the version in this instance of the PMC. Equal is denoted by a green equal mark icon, otherwise a red NOT equal sign is displayed.

    5.  Last date/time this project was published to the production server

    6.  Last operator who published this project to the production server

    7.  The group as designated by the signature on the QVW

    8.  The description of the project. This description is the same as will be displayed on the web portal.

Note: HCIPRJ is a legacy name for projects only surfaced in the PMC project grid.


### Working with Projects

By right clicking on a project (HCIPRJ) file, a context menu is displayed that allows modification and/or removal of the project file. This menu displays as![](Images/PRM_Project_Management_Console_User_Guide/image6.png ""){width=100% height=auto}\


1.  Delete – removes the project file and all associated content. Information to delete the project is relayed to the production server, but content is not deleted until the production garbage collector is executed (nightly).

2.  Copy – copies the project file to the clip board for later pasting content. *Copy/paste functionality* *has been deprecated and will be removed from future versions of the PMC due to the possible issues of creating multiple projects with conflicting information (signature of QVW conflicts with project settings).*

3.  Diff Local/Server Projects – clicking this entry will display a window that reveals the details of the production version of this project relative to the local version located in this instance of the PMC. See Context Item - Diff Local/Server Projects section for more details.

4.  Publishing History – displays a window that shows past date/times this project has been pushed to the production machine. See Context Item - Publishing History for more details.

5.  Edit Project Settings – displays a window that allows setting of various project related settings, such as description, tooltip and launch icon. For details see Context Item – Edit Project Settings.

6.  Reset Project – clicking this item will reset the local instance of this project in the PMC to be the same as the project on the production server. This item is typically used when publishing content, and is determined the content is not ready to be published for end-user access (failed validation and/or other errors) and thus need to be rolled-back in this instance of the PMC.

7.  Re-populate QVW – this entry allows an updated QVW (same signature) and user guide to be associated with the project for publication. For the scenario of limiting data access per user, report reduction is carried out to produce multiple reports associated with user accounts. For more details see Context Item – Re-populate QVW

8.  Push To Production – if the project is ready to be pushed to production, this menu item will display the summary as to the status of the project, and allow the project to be updated. For more information see Context Item – Push to Production.


### Context Item - Diff Local/Server Projects

This menu item provides project level details as to the differences between the production server version of the project and the local version in this instance of the PMC. The display appears as

![](Images/PRM_Project_Management_Console_User_Guide/image7.png ""){width=100% height=auto}\

Items that are different between production server and local versions will display a red NOT equal icon

![](Images/PRM_Project_Management_Console_User_Guide/image8.png ""){width=8% height=auto}\ .


### Context Item - Publishing History

The menu item will display a list of the last date/time this project was published to the production server along with the user’s account name that published.

![](Images/PRM_Project_Management_Console_User_Guide/image9.png ""){width=100% height=auto}\


### Context Item – Edit Project Settings

This menu item allows setting of basic project attributes, many of which are visible on the web portal for launching the report. For legacy support, this interface does support creating/deleting groups manually and manual association of a project to a group. This functionality can result in inappropriate behavior, thus it is important to understand the details of how a project, QVW, and group should be related before modifying.

![](Images/PRM_Project_Management_Console_User_Guide/image10.png ""){width=100% height=auto}\


1.  Project Name – the name of the project, this value is read-only and set when the project is initially created in the system.

2.  QVW Name – this name of the QVW once associated to the project will be changed to this value. This value is read-only and should always match the name of the project.

3.  Project Description – This description will be available/visible on the web portal page describing the content and/or analysis capabilities of the report(QVW).

4.  Project Notes – any notes required associated with this project – this information is NOT displayed on the web portal and is private to the project.

5.  Project Tooltip – this entry allows setting of a custom tooltip on the web portal. Once the user moves the mouse over the launch icon for this report(QVW), this text will be displayed. A value for this entry is optional, but highly encouraged.

6.  Project Thumbnail – this icon will be used as the launch icon for the report(QVW). Support of animated GIFs and transparent GIF/PNGs is available.

7.  User Guide - This control will allow an **optional** user guide to be uploaded and associated with the project. The user guide must be a self-contained entity, HTML with inlined images, PDF, Word or image file. A download link will be associated with the launch icon in the web portal to download/view the user guide.

8.  Group Creation/Group Deletion – to support legacy operations, the interface will allow groups to be created and deleted on-the-fly. Once a group is created or delete, this action takes place immediately on both the PMC hosting server and production. Use this functionality carefully as it is possible to remove active groups that represent reports end-users are currently using.

9.  Group association – to support legacy operations, the interface will allow an alternate group to be selected for association of the project. However changing groups for an existing report, will most likely result in not being able to update the report content due to signature/group conflicts. It is recommend the default values provided when the interface is launched should not be changed, unless you are certain as the ramifications of attempting the change.

10.  Save – persists the project to disk. Changes made by this update are confined to the PMC host machine only. No changes are made to the project on production until the project is published.


### Context Item – Re-populate QVW

This menu item allows a new QVW to be associated with the project and based on project settings may attempt to reduce reports based on user selection criteria made by a client administrator. Not all projects require reduction. The interface will also allow an optional user guide to be uploaded and associated to the project.

![](Images/PRM_Project_Management_Console_User_Guide/image11.png ""){width=100% height=auto}\


1.  New Report – the signed QVW to be associated with this project. Allowing unsigned and/or wildcard signed QVWs are no longer allowed. Once a QVW is uploaded, its signature will be verified against the project settings to ensure the QVW is appropriate to be uploaded for the group (i.e. users).

2.  User Guide – an optional user guide may be uploaded. The upload item is required to be a self-contained user guide as PDF, HTML with inline images, Microsoft Word document or image file.

3.  Download items have been depreciated and will be removed in a future version of the PMC.

4.  Update Project – verify the upload content against the project settings and begin any processing that is required to ready the project for production upload. Content will not be pushed to the production server without reviewing the summary information for the processing. See Project Processing section.


### Context Item – Push to Production

This menu item is used when a project has been updated, but for some reason was not published to the production server. Sometimes this is the result of continued review of the report before publishing, there is a timeline for when reports are pushed and the date/time has not arrived, or minor changes to the project (like description) have been made. In the event this menu item is clicked and no changes have been made to upload, you will receive the message:

![](Images/PRM_Project_Management_Console_User_Guide/image12.png ""){width=70% height=auto}\

However if changes have been made, the project summary screen will be displayed to illustrate the modifications that have been made. From the summary screen, you can instigate pushing the project to the production server. For more information on the project summary, see the Project Summary section.


### Project Processing

Project processing falls into two categories. Projects that support limiting data access per user, and reports that allow all users to view all data – thus not requiring extra processing. Setup of a project to support one of these modes is typically done automatically if the project was created by using the PMC “Import” functionality. If the project was created manually, care must be taken the project and QVW signature do not provide conflicting information. By default the system will select the strictest setting such that a minimal set of information is displayed to users in the event of a conflict. As a best practice it is recommended to ALWAYS use “Import” to setup a project to avoid these issues.

For a project that does not require the extra processing for report reduction, the project summary screen is displayed showing that reduction is “OFF”. At this point the project can be pushed to the production machine via the project summary screen. For more information see the Project Summary section.

![](Images/PRM_Project_Management_Console_User_Guide/image13.png ""){width=100% height=auto}\

For a project that is signed to limit data access per user, extra processing must be run to reduce the uploaded “Master” QVW into sub reports with limited data. Each sub report with limited data is then associated with the appropriate user account. Selections as to what a user can access data-wise in the report is set by a client administrator for the user. Once the “Update Project” button is clicked on the Re-populate QVW window, the system will

1.  Download all the selections made for each user

2.  Download all the QVW names as paired with each users account (must be kept same to minimize impact on QV bookmarks)

3.  Generates a reduced report from the Master QVW, using the selections for the user

4.  Verifies processing errors did not occur in reduction ( data driven process)

5.  Generates summary data for the complete reduction process that includes – selections for users that could not be reduced on, since they no longer appear in new data set, selection options in the new data set that are not present in old data set (new selection options).

6.  Once the summary is reviewed and accepted, the project and be published to the production server via the project summary screen. See Project Summary section.


### Project Summary

The project summary screen is composed of a tabbed view. The “Summary” tab provides information on

1.  Project Name

2.  Project Description

3.  Project Thumbnail

4.  Project User Guide

5.  Publication Directory on production machine

6.  Group published to

7.  QVW upload date/tome

8.  QVW uploaded by

9.  Report Reduction - “Off” or “QVW reduced for X of Y users”

10.  Project last update date/time

11.  Project last updated by

The “Failed Selections per User” tab, provides a view of each user’s account associated with selections made for the account, but specific selections were not found in the new data set. Even though listed as “Failed” this is a valid state in a data driven system of this type.

The “New Selectable Items” tab, provides a list of all the values found in the new data set that were not available in the old data set. Thus no users will have these values as selections for their account (yet).

The "Reduction status" tab provides two lists.  One list contains all the users that had reports successfully reduced, and the other list contains users that failed to have a report reduced, and thus do not have a QVW to view via the web portal.

The "general processing" tab provides basic trace information on system activity as reduction process is executing (logs).

![](Images/PRM_Project_Management_Console_User_Guide/image14.png ""){width=100% height=auto}\

![](Images/PRM_Project_Management_Console_User_Guide/image15.png ""){width=100% height=auto}\

At the bottom of the window is the “Publish to Production” button, once clicked all content is made available to end-users. For more information on Publish to Production functionality, see Project Publishing section.


### Project Publishing

Once the project has been reviewed and accepted, clicking the “Publish to Production” button will publish the content to the PRM production server. As content is updated, a status is displayed on upload progress. Once the process is complete, end-users have access to the report and/or reduced reports depending upon project settings. At completion of publishing, an email is dispatched to PRM support along with project summary information that a project was published to production.

![](Images/PRM_Project_Management_Console_User_Guide/image16.png ""){width=50% height=auto}\
