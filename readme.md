# Qlikview Hosting

This repository will hold the majority of the solutions that support the PRM Analytics Products.  Especially those focused on web applications or Microsoft technologies.

Actual solution files (and all their entourage) should be stored in subfolders under the appropriate root folder (e.g. `/Apps/CDR` or `/WebApps/MillFrame`).

## Solutions in this repository

**Millframe** - The main Millframe solution is located at [`/WebApps/Millframe/MillFrame.sln`](https://indy-github.milliman.com/PRM/qlikview-hosting/blob/develop/WebApps/MillFrame/MillFrame.sln). This solution contains multiple projects related to the main web solution publicly hosted at https://prm.milliman.com.

**SystemReporting** - The SystemReporting solution contains multiple projects related to retrieving and loading logs from the Qlikview server into a database for analysis. The solution is located at [`/Apps/SystemReporting/SystemReporting.sln`](https://indy-github.milliman.com/PRM/qlikview-hosting/blob/develop/Apps/SystemReporting/SystemReporting.sln)

## Millframe components

Each Millframe component is represented by a project under the broader Millframe solution.

|Component|Visual Studio Project|Description|
|---------|-------|-----------|
|Web Portal|Milliman|The public-facing web application accessed by end users of the PRM solution.|

## SystemReporting components

System reporting components are represented by projects under the SystemReporting solution.
