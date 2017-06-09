ROBOCOPY Milliman ../PRM_Staging/Milliman /MIR
powershell -Command "(gc ../PRM_Staging/Milliman/web.config) -replace 'appsettings_PreStag', 'appsettings_Stag' | Out-File ../PRM_Staging/Milliman/web.config"
powershell -Command "(gc ../PRM_Staging/Milliman/web.config) -replace 'connectionStrings_PreStag', 'connectionStrings_Stag' | Out-File ../PRM_Staging/Milliman/web.config"
