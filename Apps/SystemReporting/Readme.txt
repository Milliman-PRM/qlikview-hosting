
/*******************************************************************************************************/
This program depends on following directories for processing of log files:

<!--GetFileSourceLocation - source-->
<add key="SourceDir" value="C:\ProductionLogs\" />
<add key="IISLogsS" value="C:\ProductionLogs\IISLogs\" />
<add key="MiscLogsS" value="C:\ProductionLogs\MiscLogs\" />
<add key="QVAuditLogsS" value="C:\ProductionLogs\QVLogs\" />
<add key="QVSessionLogsS" value="C:\ProductionLogs\QVLogs\" />

/////////////-----------TODO: we will change the directory C to D --------------/////////////
/*******************************************************************************************************/
This program will log the sucessfully "Processed" file on this location.


  <!--GetFileCopyToDestinationInLocation - In-->
  <add key="ProcessingInDir" value="C:\LogFileProcessor\IN\" />

  Note: Note: The file is moved temporarily to this location and after completion, its deleted
/*******************************************************************************************************/
This program will log the sucessfully processed file to a file ["ProcessedFileLog"] on this location.

<!--ProcessedFileLog-->
<add key="ProcessedFileLogDirectory" value="C:\LogFileProcessor\BackUp\" />
<add key="ProcessedFileLogFileName" value="ProcessedFileLog" />

/*******************************************************************************************************/
This program will compare the files documented in the status file ["SyncStatus"] on this location.
This file gives a status of file to be processed and that file name will be matched against the files
on the source location to ensure that certian file is not processed twice.

<!--Filestatus - this is file that provides the file name to be processes-->
<add key="statusFileName" value="status" />
<add key="FileStatus" value="C:\ProductionLogs\SyncStatus\" />


/*******************************************************************************************************/
This program will create "Exception log" file ["Logger"] at this location:

<!--Logger-->
<add key="LoggerFileDirectory" value="C:\LogFileProcessor\" />
<add key="LoggerFileName" value="Logger" />
