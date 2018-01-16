## Release Notes

A non-exhaustive list of what has changed in a more readable form than a commit history.

### v4.2.5

  - Remove the **Reset** button from the User Profile Password Settings so that our end users and not confused about which button performs which action

### v4.2.4

  - Fix the table formatting in the reset password email

### v4.2.3

  - Update the welcome email and reset password email to users on PRM so that they are more generic for users from other offices and cleaned up some wording and formatting.
  - Remove time.aspx

### v4.2.2

#### Client visible changes (e.g. Portal - prm.milliman.com, User guide)

  - None

#### Lower level changes( e.g. Milliman Common, Project Management Console Services, Milliman Services, Databases)

  - Enhanced exception logging in nightly backup utility

### v4.2.1

#### Client visible changes (e.g. Portal - prm.milliman.com, User guide)

  - Fixed an error that occurs when saving password changes in some circumstances 
  - Corrected minimum password length enforcement from 9 to 8 characters
  - Changed all applications to display only first 2 components of software version

#### Lower level changes( e.g. Milliman Common, Project Management Console Services, Milliman Services, Databases)

  - Changed default password expiration from 60 to 90 days
  - Improved log file readability

### v4.2.0

#### Client visible changes (e.g. Portal - prm.milliman.com, User guide)

  - Enforce end user password expiration
  - Client Publisher Console user guide updated
  - Improved the wording of warning messages in the Client Publisher
  - Add a button to the Project Management Console to view summary stats before publishing
  - Blocked autocompletion of password fields for security purposes
  - The validation message for invalid username and passwords was updated to be less specific

#### Client Admin changes (e.g. Client Publisher, Client Administration Console - prm.milliman.com/PRMAdmin)

  - Added new authorization checks to the Client User Admin and Client Publisher

#### PRM Admin changes (e.g. Project Management Console, Signature App, User Admin)

  - Improvements to the reduction status reporting
  - Improvements to the reduction process error handling
  - Blocked autocompletion of password fields for security purposes
  - Force redirect to login.aspx if the user is not currently authenticated
  - Removed the hex encoded user passwords from the user profile view
  - Removed the security question from the user profile view

#### Automated Processes changes (e.g. License Cleaner, Report Reduction, System Backup, Server Monitor, Backup Utility)

  - Added explicit hard-coded directory creation in prebuild script to facilitate CI testing
  - Enhanced the automated reclamation of unused QlikView Document and named user CALs

#### Lower level changes( e.g. Milliman Common, Project Management Console Services, Milliman Services, Databases)

  - Removed hard-coded usernames and email addresses from production-deployed code
  - Fixed a casing issue in the password reset files
  - Updated the way that database connections are executed
  - Deployment scripts added to the repository
  - Removed UTF-8 encoding from the web.config to prevent issues with IIS
  - Improved documentation (Readmes, Wiki, Release Notes)
  - Removal of Clinical Data Repository (CDR) Components from the code base
  - Fixed the log4net reference
  - Added `CI_Publish` publishing definition to support automated deployments of in-progress branches to a test environment
  - All open pull requests are published to PRM2, and cleaned up once closed.
  - Implemented x-frame-options HTTP header to defend against input hijacking and impersonation attacks
  - Internet Explorer is no longer forced to render in IE7 compatibility mode
  - All cookies now have the require SSL flag set to true




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
