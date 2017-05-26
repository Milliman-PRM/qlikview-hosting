# Upgrade checklist template

This document is a template, to be copied and adjusted for all releases. The document should be stored in this location in the repository for future reference.

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
-   Back up the `PortalDBSFV2m1` to another location
-   Archive the two backups made above to `\\indy-backups\PRM\_Archive`
