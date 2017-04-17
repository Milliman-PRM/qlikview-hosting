# Qlikview Hosting

This repository will hold the majority of the solutions that support the PRM Analytics Products.  Especially those focused on web applications or Microsoft technologies.

Actual solution files (and all their entourage) should be stored in subfolders under the appropriate root folder (e.g. `/Apps/CDR` or `/WebApps/MillFrame`).

## Solutions in this repository

**Millframe** - The main Millframe solution is located at [`/WebApps/Millframe/MillFrame.sln`](https://indy-github.milliman.com/PRM/qlikview-hosting/blob/develop/WebApps/MillFrame/MillFrame.sln). This solution contains multiple projects related to the main web solution publicly hosted at https://prm.milliman.com.

**SystemReporting** - The SystemReporting solution contains multiple projects related to retrieving and loading logs from the Qlikview server into a database for analysis. The solution is located at [`/Apps/SystemReporting/SystemReporting.sln`](https://indy-github.milliman.com/PRM/qlikview-hosting/blob/develop/Apps/SystemReporting/SystemReporting.sln)

**Milliman** - Located at [`/Apps/ReductionServer/Milliman.sln`](https://indy-github.milliman.com/PRM/qlikview-hosting/blob/develop/Apps/ReductionServer/Milliman.sln).  Contains projects comprising the new reduction service to be used by the client publisher console.

**RedoxFeedHandler** - A clinical data feed handler for use of Redox 3rd party service integration.  This project was abandoned and the code is not complete. It is good enough to demonstrate proof of principal but not downstream utility.  Candidate for removal from this repo since the work was transferred to the CDR repo.  

**WoahRawDataExtraction** - Daily extraction of raw data from Bay Clinic. The solution contains projects for deployment as a GUI (for interactive use) and a console application as well as a system service as a growth capability.  Candidate for removal from this repo since the work was transferred to the CDR repo.  

**WoahCdrAggregation** - Data aggregation to a common PostgreSQL schema supporting extracted data from Bay Clinic and North Bend Medical Center.  Candidate for removal from this repo since the work was transferred to the CDR repo.  

**NbmcUnity** - Data extraction from North Bend Medical Center.  Candidate for removal from this repo since the work was transferred to the CDR repo.  

**MillframeConfigComparison** - A developer's GUI utility program to compare application config files for any application.  Highlights value differences, presence of different keys, and (optionally) evaluates presence of required keys.  

**PasswordResetUtility** - Probably just what it sounds like.  

**Milliman** - The new reduction server.  Works but has some likely multithreading issues that cause task failures.  Work in progress.  

**SystemReporting** - Extraction of PRM end user statistics and persistence to a database.  

**DcrDbLib** - Contains a C# project to wrap the CDR database in PostgreSQL.  Candidate for removal from this repo since the work was transferred to the CDR repo.  

**MillimanUserAdmin** - Appears to be a solution containing only the MillimanUserAdmin project.  Likely created automatically when someone tried to open the contained project outside the Millframe solution.  If so, this solution file should be deleted **but not the contained project files**.  

**CLSMedicareReimbursement** - Appears to be part of the Roche effort since it is located in this path.  Maybe someone else knows what it does.  

### Millframe components

Each Millframe component is represented by a project under the broader Millframe solution.

|Component|Visual Studio Project|Description|URL/Deployment Location|
|---------|-----|-----------|----|
|Web Portal|Milliman|The public-facing web application accessed by end users of the PRM solution.|prm.milliman.com|
|Signature tool|MillimanSignature|A utility to view QVW signature information and sign QVWs with metadata used in publishing|N/A|
|Global user admin|MillimanUserAdmin|A user administration portal used to manage users in all groups|prm.milliman.com/admin|
|Client User Admin|ClientUserAdmin|Client-facing user administration portal for groups to manage their own users| prm.milliman.com/PRMClientAdmin/ <br><br> *Note: this link will bring user to '403 - Forbidden' page. Must access through prm.milliman.com* |
|Client Publisher|MillimanClientPublisher|Currently in development. A user-facing utility for publishing reports and updates.|?|
|Project Management Console (PMC)|MillimanProjectManConsole|Utilized to publish and update QVWs/projects to the production server.|indy-ss01.milliman.com/PRMPMC/|
|PMC Web Services|MillimanProjectManagementConsoleServices|A SOAP web service utilized by the PMC|prm.milliman.com/prm_projectmanagementservices|
|Milliman Common|MillimanCommon|Shared code library utilized by various projects. Need to replace custom build step.  Copy dependency dll during build of depending project.||
|QV License Cleaner|MillimanQVLicenseCleaner|GUI and Console application to clean up assigned Qlikview licenses. QV Server doesn't do this as we intend. This utility implements what we need| N/A|
|Report Reduction Library|MillimanReportReduction|Library including functionality used in reducing reports|Not part of the new reduction server|
|Service App Harness|MillimanServiceAppHarness|A legacy test tool for SAML. *This can be removed.*|N/A|
|?|MillimanServices|?|?|
|Web test harness|MillimanServiceWEBTestHarness|Legacy testing tool for the User API, which is unused. *This can be removed.*|N/A|
|SQL Browser|MillimanSQLBrowser|Was intended to be a web interface to the Power User DataMart, but was not deployed or used. *This can be removed.*|N/A|
|Support|MillimanSupport|Used to display server health. Will likely be obsoleted by Zabbix in the near future.|/Help (on all web servers)|
|System Backup|MillimanSystemBackup|Creates a backup of the production system. Runs as a scheduled task on Indy-PRM-1.|Works in conjunction with a task on PRM2.|
|Validation GC|MillimanValidationGC|Legacy license cleaner. *This can be removed.*|N/A|
|Server Monitor|PrmServerMonitor|GUI to support production from an IT perspective. Currently removed orphaned Qlikview tasks.|Structured for future expansion.|
|Reporting library|ReportingCommon|Shared code for log parsing. Possibly outdated and removable.|?|

### System Reporting components

System reporting components are represented by projects under the SystemReporting solution.


|Component|Visual Studio Project|Description|URL/Deployment Location|
|---------|-------|-----------|---|
||Framework/Milliman.Controller|Defines controller classes for various log file types|N/A|
||Framework/Milliman.Data|Defines database connectivity|N/A|
||Framework/Milliman.Entities|Data model classes for various log types|N/A|
||Framework/Milliman.Service|?|N/A|
||Framework/Milliman.Utilities|Defines various convenience classes for use in other projects in the solution|N/A|
||FileProcessor|Processes the contents of Qlikview and IIS log files|?|
||FileProcessorApplication|A console application wrapper for FileProcessor|On `Indy-PRM-2`, `D:\PRMReportingSyncDaemon\LogFileProcessor`|
||ReportFileGenerator|Contains functionality to generate Excel & CSV reports of log file entries. It's unclear whether this has ever been used.|N/A|
||ReportFileGeneratorApplication|Console wrapper for ReportFileGenerator|N/A|
||ReportingCommon|?|N/A|
||ReportingTest|Defines basic test cases for some of the projects in the solution. Very incomplete.|?|
||SystemReportingWinUI|GUI wrapper for ReportFileGenerator|N/A|

### Roche / Clinical Lab Systems (CLS) Handbook components

|Component|Visual Studio Project|Description|URL/Deployment Location|
|---------|-----|-------------|---|
|Business logic|ClSBusinessLogic|?|?|
|Configuration|CLSConfiguration|?|?|
|Configuration common|CLSConfigurationCommon|?|?|
|?|CLSConfigurationProductionDaemon|?|?|
|?|CLSConfigurationServices|?|?|
|?|CLSMedicareReimbursement|The actual user interface|Indy-Roche01 E:\InstalledApplications\CLSMedicareReimbursement|
|?|CLSTest|?|?|
