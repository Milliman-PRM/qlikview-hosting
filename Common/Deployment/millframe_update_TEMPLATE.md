# Upgrade checklist template

This document is a template, to be copied and adjusted for all deployments. The document should be stored in this location in the repository for future reference.

Review the release notes for the new Millframe version you will be installing, then determine whether any additional testing/verification steps need to occur during or after the upgrade.

## System Prep/Verification tasks

The following procedures must be completed before deployment begins.

### System Cleaning – `Indy-PRM-1`

-   Remove all files older than 14 days from the Cache directory
-   Remove empty sub-directories from DBBrowserCache
-   If the log file under each of these directories is larger than 10 MB, archive the file to a new location and remove it
    -   `Milliman\Logs`
    -   `PRMClientAdmin\Logs`
    -   `PRMClientPublisher\Logs`
-   Archive and remove zipped history files under `QVDocuments`
-   Delete all files from `C:\ProgramData\QlikTech\SourceDocuments`
    -   These are temporary copies of reduced QVWs and are unused. Any remaining artifacts should be cleaned up
-   Run Windows Disk Cleanup

### System Backup Procedure - `Indy-PRM-1`
-   Make sure no users are running the system
-   Reboot the server
-   Copy `D:\InstalledApplications\PRM` to another location
-   Back up the `PortalDBSFV2m1` database to another location
-   Archive the two backups made above to `\\indy-backups\PRM\_Archive`

### Deploying code updates - Executed from `Indy-PRM-2`
-	Make sure the pre-release branch's CI deployment is up-to-date with the release code. This will be the folder from which code is copied for the release.
-	Map `\\indy-prm-1\d$\InstalledApplications\PRM` as a network drive. (Drive letter X is preferred, as it's the default used in the template `pathMap.csv` file.)
-	Update the `pathMap.csv` file with the correct source and target paths. 
	-	Take care to ensure that your local copy of the file only specifies source & target paths for applications to be updated on the target server. For example, do not include the PMC when deploying to the production web server, as it should not have that application installed.
	-	If you have mapped the share above as X:, you will only have to update the source folder paths. Find & replace is your friend.
-	Execute `Millframe_Deployment.ps1`, specifying your updated `pathMap.csv` as the `-pathMapFile` parameter, to complete the code deployment.
-	Restart IIS on the production server

### Post-deployment testing

- Test logging in to all updated applications
- Test that users can still access reports and user guides in the Web Portal
- Examine all web.config files in updated folders. Ensure that the following references were updated correctly:
  - Any references to appsettings_CI.config have been changed to appsettings_PROD.config
  - Any reference to connectionstrings_CI.config have been changed to connectionstrings_PROD.config
  - Any databse connection strings point at the database named PortalDBSFV2m1 and not PortalDB_CI
