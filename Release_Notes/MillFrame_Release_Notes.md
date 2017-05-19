## Release Notes

A non-exhaustive list of what has changed in a more readable form than a commit history.

### v4.2.0

#### Client visible changes (e.g. Portal - prm.milliman.com, User guide)

  - Enforce end user password expiration
  - Client Publisher Console user guide updated
  - Improved the wording of warning messages in the Client Publisher
  - Add a button to the Project Management Console to view summary stats before publishing

#### Client Admin changes (e.g. Client Publisher, Client Administration Console - prm.milliman.com/PRMAdmin)

  - *None*

#### PRM Admin changes (e.g. Project Management Console, Signature App, User Admin)

  - Improvements to the reduction status reporting
  - Improvements to the reduction process error handling

#### Automated Processes changes (e.g. License Cleaner, Report Reduction, System Backup, Server Monitor, Backup Utility)

  - Added explicit hard-coded directory creation in prebuild script to facilitate CI testing.
  - Enhanced the automated reclaimation of unused Qlikview Document and named user CALs.  

#### Lower level changes( e.g. Milliman Common, Project Management Console Services, Milliman Services, Databases)

  - Removed hard-coded usernames and email addresses from production-deployed code.
  - Fixed a casing issue in the password reset files
  - Updated the way that database connections are executed
  - Deployment scripts added to the repository
  - Removed UTF-8 encoding from the web.config to prevent issues with IIS
  - Improved documentation (Readmes, Wiki, Release Notes)
  - Removal of Clinical Data Repository (CDR) Components from the code base
  - Fixed the log4net reference
  - Added `CI_Publish` publishing definition to support automated deployments of in-progress branches to a test environment.



### v4.1.1

#### Client visible changes (e.g. Portal - prm.milliman.com, User guide)

  - *None*

#### Client Admin changes (e.g. Client Publisher, Client Administration Console - prm.milliman.com/PRMAdmin)

  - Added a "Restart Project Update" button to the final screen of the Client Publisher

#### PRM Admin changes (e.g. Project Management Console, Signature App, User Admin)

  - *None*

#### Automated Processes changes (e.g. License Cleaner, Report Reduction, System Backup, Server Monitor, Backup Utility)

  - *None*

#### Lower level changes( e.g. Milliman Common, Project Management Console Services, Milliman Services, Databases)

  - Allow code to reconnect to running tasks
