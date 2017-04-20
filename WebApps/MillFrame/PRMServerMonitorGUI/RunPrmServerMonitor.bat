REM /*
REM  * CODE OWNERS: Tom Puckett, 
REM  * OBJECTIVE: Batch file for launching the PrmServerMonitor application non-interactively.
REM  * DEVELOPER NOTES: The `start /nowait` launch mechanism causes the console from which the application is launched to block pending program completion
REM  */

start /wait PrmServerMonitorGUI.exe console orphantaskremoval managecals reportnamedcalassigned reportnamedcallimit reportdocumentcalassigned reportdocumentcallimit
