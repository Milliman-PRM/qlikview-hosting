ROBOCOPY PRMAdmin ../PRM_Staging/PRMAdmin /MIR
powershell -Command "(gc ../PRM_Staging/PRMAdmin/web.config) -replace 'appsettings_PreStag', 'appsettings_Stag' | Out-File ../PRM_Staging/PRMAdmin/web.config"
powershell -Command "(gc ../PRM_Staging/PRMAdmin/web.config) -replace 'connectionStrings_PreStag', 'connectionStrings_Stag' | Out-File ../PRM_Staging/PRMAdmin/web.config"
