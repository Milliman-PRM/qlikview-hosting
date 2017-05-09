C:\Windows\SysWOW64\inetsrv\appcmd.exe add app /site.name:"Default Web Site" /path:"/prm_ci_<<branch_name>>_ClientPublisher" /physicalPath:"D:\installedapplications\prm_ci\<<branch_name>>\ClientPublisher" /applicationPool:"Qlikview IIS" 

C:\Windows\SysWOW64\inetsrv\appcmd.exe add app /site.name:"Default Web Site" /path:"/prm_ci_<<branch_name>>_ClientUserAdmin" /physicalPath:"D:\installedapplications\prm_ci\<<branch_name>>\ClientUserAdmin" /applicationPool:"Qlikview IIS"

C:\Windows\SysWOW64\inetsrv\appcmd.exe add app /site.name:"Default Web Site" /path:"/prm_ci_<<branch_name>>_Milliman" /physicalPath:"D:\installedapplications\prm_ci\<<branch_name>>\Milliman" /applicationPool:"Qlikview IIS" 

C:\Windows\SysWOW64\inetsrv\appcmd.exe set config /commit:WEBROOT /section:system.web/authentication /forms.loginURL:UserLogin.aspx

C:\Windows\SysWOW64\inetsrv\appcmd.exe add app /site.name:"Default Web Site" /path:"/prm_ci_<<branch_name>>_MillimanServices" /physicalPath:"D:\installedapplications\prm_ci\<<branch_name>>\MillimanServices" /applicationPool:"Qlikview IIS"  

C:\Windows\SysWOW64\inetsrv\appcmd.exe add app /site.name:"Default Web Site" /path:"/prm_ci_<<branch_name>>_MillimanSupport" /physicalPath:"D:\installedapplications\prm_ci\<<branch_name>>\MillimanSupport" /applicationPool:"Qlikview IIS" 

C:\Windows\SysWOW64\inetsrv\appcmd.exe add app /site.name:"Default Web Site" /path:"/prm_ci_<<branch_name>>_ProjectManagementConsoleServices" /physicalPath:"D:\installedapplications\prm_ci\<<branch_name>>\ProjectManagementConsoleServices" /applicationPool:"Qlikview IIS" 

C:\Windows\SysWOW64\inetsrv\appcmd.exe add app /site.name:"Default Web Site" /path:"/prm_ci_<<branch_name>>_ProjectManConsole" /physicalPath:"D:\installedapplications\prm_ci\<<branch_name>>\ProjectManConsole" /applicationPool:"Qlikview IIS" 

C:\Windows\SysWOW64\inetsrv\appcmd.exe add app /site.name:"Default Web Site" /path:"/prm_ci_<<branch_name>>_SQLBrowser" /physicalPath:"D:\installedapplications\prm_ci\<<branch_name>>\SQLBrowser" /applicationPool:"Qlikview IIS" 

C:\Windows\SysWOW64\inetsrv\appcmd.exe add app /site.name:"Default Web Site" /path:"/prm_ci_<<branch_name>>_WEBTestHarness" sGIT_BRANCH%\WEBTestHarnessphysicalPath:"D:\installedapplications\prm_ci\sGIT_BRANCH%\WEBTestHarness" /applicationPool:"Qlikview IIS" 

echo %errorlevel% > error.log
