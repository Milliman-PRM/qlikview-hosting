# Qlikview Server Update Process

## License Verification
-   Verify Qlikview Server license
    -   Open QV Management Console
    -   Navigate to System tab
    -   Navigate to Licenses sub-tab
    -   Select the QlikView server item in the list
    -   In right hand panel select the “Qlikview OEM Server License” tab
    -   Verify a valid license shows in the text box containing the LEF information
-   Verify Qlikview Publisher license
    -   Open QV Management Console
    -   Navigate to System tab
    -   Navigate to Licenses sub-tab
    -   Select the QlikView publisher item in the list
    -   Verify a valid license shows in the text box containing the LEF information
-   Verify Qlikview has an end-user license report available for review
    -   Open QV Management Console
    -   Navigate to System tab
    -   Navigate to Licenses sub-tab
    -   Select the QlikView server item in the list
    -   In right hand panel select the “Client Access Licenses” tab
    -   Verify the section with “Named User Cals” shows there are un-assigned license
    -   Verify the section with “Document CALs” shows there are un-assigned license

## Update process
-   Verify no users have active Qlikview sessions
    -   Open QV Management Console
    -   Navigate to QVS Statistics/Active Users
-   Stop all Qlikview services
-   Install update
-   Reboot server
-   Verify status of Qlikview Server
    -   Open QV Management Console
    -   Navigate to Status/Services
-   Verify Qlikview server status by reviewing system logs
    -   Use Event Viewer on the system to see if any errors occurred while the services were starting up
-   Verify Qlikview reports can be launched from the web portal
-   Verify Qlikview reports can be reduced
